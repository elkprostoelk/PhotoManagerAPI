using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhotoManagerAPI.Core.Configurations;
using PhotoManagerAPI.Core.DTO;
using PhotoManagerAPI.DataAccess.Entities;
using PhotoManagerAPI.DataAccess.Repositories;

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

        var fileUploadResult = await _pictureUploaderService.UploadAsync(new UploadPictureDto
        {
            File = newPictureDto.File,
            FileName = newPictureDto.FileName,
            UserId = newPictureDto.UserId
        });

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
}