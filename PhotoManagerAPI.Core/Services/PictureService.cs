using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhotoManagerAPI.Core.Configurations;
using PhotoManagerAPI.Core.Constants;
using PhotoManagerAPI.Core.DTO;
using PhotoManagerAPI.DataAccess.Entities;
using PhotoManagerAPI.DataAccess.Repositories;
using System.Linq.Expressions;

namespace PhotoManagerAPI.Core.Services;

public class PictureService : IPictureService
{
    private readonly IPictureUploaderService _pictureUploaderService;
    private readonly IRepository<Picture, Guid> _pictureRepository;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ImageOptions _imageOptions;
    private readonly ILogger<PictureService> _logger;
    private readonly IMapper _mapper;

    public PictureService(
        IRepository<Picture, Guid> pictureRepository,
        IUserRepository userRepository,
        IConfiguration configuration,
        ILogger<PictureService> logger,
        IOptions<ImageOptions> imageOptions,
        IPictureUploaderService pictureUploaderService,
        IMapper mapper)
    {
        _pictureRepository = pictureRepository;
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
        _imageOptions = imageOptions.Value;
        _pictureUploaderService = pictureUploaderService;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<ShortPictureDto>> SearchAsync(
        SearchPicturesDto searchPicturesDto,
        CancellationToken cancellationToken)
    {
        var picturesQuery = _pictureRepository
            .EntitySet
            .AsNoTracking()
            .AsQueryable();

        picturesQuery = ApplyFilters(picturesQuery, searchPicturesDto);

        picturesQuery = ApplySorting(picturesQuery, searchPicturesDto);

        var totalCount = await picturesQuery.CountAsync(cancellationToken);
        var pagesCount = Convert.ToInt32(Math.Ceiling((float)totalCount / searchPicturesDto.ItemsPerPage));

        return new PagedResultDto<ShortPictureDto>
        {
            ItemsPerPage = searchPicturesDto.ItemsPerPage,
            Page = searchPicturesDto.Page,
            PageCount = pagesCount,
            TotalItems = totalCount,
            PageItems = await GetShortPictureDtoListAsync(picturesQuery, searchPicturesDto, cancellationToken)
        };
    }

    public async Task<ServiceResultDto> AddAsync(NewPictureDto newPictureDto)
    {
        var userExists = await _userRepository.ExistsAsync(newPictureDto.UserId);
        if (!userExists)
        {
            _logger.LogWarning(
                "Cannot find a user with ID {UserId}. Unable to add a picture.",
                newPictureDto.UserId);
            
            return new ServiceResultDto
            {
                IsSuccess = false,
                Errors = ["User was not found."]
            };
        }

        if (!ValidateFile(newPictureDto, out var validationErrors))
        {
            return new ServiceResultDto  
            {
                IsSuccess = false,
                Errors = validationErrors
            };
        }

        var fileUploadResult = await _pictureUploaderService.UploadAsync(
            _mapper.Map<UploadPictureDto>(newPictureDto));

        if (!fileUploadResult.IsSuccess)
        {
            return new ServiceResultDto
            {
                IsSuccess = false,
                Errors = fileUploadResult.Errors
            };
        }

        if (string.IsNullOrWhiteSpace(fileUploadResult.Container))
        {
            return new ServiceResultDto
            {
                IsSuccess = false,
                Errors = ["Cannot add a picture. The file path is not specified."]
            };
        }

        var added = await _pictureRepository.CreateAsync(new Picture
        {
            Id = Ulid.NewUlid().ToGuid(),
            PhysicalPath = fileUploadResult.Container,
            UserId = newPictureDto.UserId,
            CameraModel = newPictureDto.CameraModel,
            DelayTimeMilliseconds = newPictureDto.DelayTimeMilliseconds,
            Description = newPictureDto.Description,
            Flash = newPictureDto.Flash,
            FocusDistance = newPictureDto.FocusDistance,
            Height = newPictureDto.Height,
            Width = newPictureDto.Width,
            Iso = newPictureDto.Iso,
            ShootingDate = newPictureDto.ShootingDate,
            Title = newPictureDto.Title,
            Created = DateTime.UtcNow
        });

        return new ServiceResultDto
        {
            IsSuccess = added,
            Errors = added ? [] : ["Failed to add a picture."]
        };
    }

    public async Task<PictureDto?> GetAsync(Guid id)
    {
        return await _pictureRepository
            .EntitySet
            .AsNoTracking()
            .Include(p => p.User)
            .ProjectTo<PictureDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    private bool ValidateFile(NewPictureDto newPictureDto, out List<string> errors)
    {
        errors = [];
        var isFileSizeValid = newPictureDto.FileSizeBytes <= _imageOptions.MaxFileSizeBytes;
        var isFileTypeValid = _imageOptions.AllowedFileTypes.Contains(newPictureDto.GetExtension());

        if (!isFileSizeValid)
        {
            _logger.LogInformation(
                    "Size of a file {FileName} is {FileSizeBytes} bytes and greater than a defined limit.",
                    newPictureDto.FileName,
                    newPictureDto.FileSizeBytes);
            errors.Add("File size is too large.");
        }

        if (!isFileTypeValid)
        {
            _logger.LogInformation(
                "A file {FileName} is not of allowed file types. Allowed types: {AllowedFileTypes}",
                newPictureDto.FileName,
                string.Join(", ", _imageOptions.AllowedFileTypes));
            errors.Add("File type is invalid.");
        }

        return isFileSizeValid && isFileTypeValid;
    }

    private async Task<List<ShortPictureDto>> GetShortPictureDtoListAsync(
        IQueryable<Picture> picturesQuery,
        SearchPicturesDto searchPicturesDto,
        CancellationToken cancellationToken) =>
        await picturesQuery
                .Include(p => p.User)
                .Skip((searchPicturesDto.Page - 1) * searchPicturesDto.ItemsPerPage)
                .Take(searchPicturesDto.ItemsPerPage)
                .ProjectTo<ShortPictureDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

    private static IQueryable<Picture> ApplyFilters(IQueryable<Picture> picturesQuery, SearchPicturesDto searchPicturesDto)
    {
        if (searchPicturesDto is null)
            return picturesQuery;

        if (!string.IsNullOrWhiteSpace(searchPicturesDto.Title))
        {
            picturesQuery = picturesQuery.Where(p => p.Title.Contains(searchPicturesDto.Title));
        }

        if (!string.IsNullOrWhiteSpace(searchPicturesDto.CameraModel))
        {
            picturesQuery = picturesQuery.Where(p => !string.IsNullOrWhiteSpace(p.CameraModel)
                && p.CameraModel.Contains(p.CameraModel));
        }

        if (searchPicturesDto.DelayTimeMilliseconds.HasValue)
        {
            picturesQuery = picturesQuery.Where(p => p.DelayTimeMilliseconds.HasValue
                && p.DelayTimeMilliseconds == searchPicturesDto.DelayTimeMilliseconds);
        }

        if (!string.IsNullOrWhiteSpace(searchPicturesDto.Description))
        {
            picturesQuery = picturesQuery.Where(p => !string.IsNullOrWhiteSpace(p.Description)
                && p.Description.Contains(p.Description));
        }

        if (searchPicturesDto.Flash.HasValue)
        {
            picturesQuery = picturesQuery.Where(p => p.Flash.HasValue
                && p.Flash == searchPicturesDto.Flash);
        }

        if (!string.IsNullOrWhiteSpace(searchPicturesDto.FocusDistance))
        {
            picturesQuery = picturesQuery.Where(p => !string.IsNullOrWhiteSpace(p.FocusDistance)
                && p.FocusDistance.Contains(p.FocusDistance));
        }

        if (searchPicturesDto.Width.HasValue)
        {
            picturesQuery = picturesQuery.Where(p => p.Width == searchPicturesDto.Width);
        }

        if (searchPicturesDto.Height.HasValue)
        {
            picturesQuery = picturesQuery.Where(p => p.Height == searchPicturesDto.Height);
        }

        if (!string.IsNullOrWhiteSpace(searchPicturesDto.Iso))
        {
            picturesQuery = picturesQuery.Where(p => !string.IsNullOrWhiteSpace(p.Iso)
                && p.Iso.Contains(p.Iso));
        }

        if (searchPicturesDto.ShootingDateFrom.HasValue)
        {
            picturesQuery = picturesQuery.Where(p => p.ShootingDate.HasValue
                && p.ShootingDate >= searchPicturesDto.ShootingDateFrom);
        }

        if (searchPicturesDto.ShootingDateTo.HasValue)
        {
            picturesQuery = picturesQuery.Where(p => p.ShootingDate.HasValue
                && p.ShootingDate <= searchPicturesDto.ShootingDateTo);
        }

        return picturesQuery;
    }

    private static IQueryable<Picture> ApplySorting(IQueryable<Picture> picturesQuery, SearchPicturesDto searchPicturesDto)
    {
        if (string.IsNullOrWhiteSpace(searchPicturesDto.SortBy))
        {
            return picturesQuery;
        }

        Expression<Func<Picture, object?>> sortingExpression = searchPicturesDto.SortBy switch
        {
            SortingFields.Title => p => p.Title,
            SortingFields.CreatedDate => p => p.Created,
            SortingFields.ShootingDate => p => p.ShootingDate,
            _ => p => p.Created
        };

        return searchPicturesDto is { SortOrder: Enums.SortOrder.Ascending }
            ? picturesQuery.OrderBy(sortingExpression)
            : picturesQuery.OrderByDescending(sortingExpression);
    }
}