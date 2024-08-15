using AutoMapper;
using PhotoManagerAPI.Core.DTO;
using PhotoManagerAPI.DataAccess.Entities;
using PhotoManagerAPI.Web.Models;

namespace PhotoManagerAPI.Web.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<NewPictureModel, NewPictureDto>()
            .ForMember(dto => dto.FileSizeBytes, opts => opts.MapFrom(m => m.File.Length))
            .ForMember(dto => dto.FileName, opts => opts.MapFrom(m => m.File.FileName))
            .ForMember(dto => dto.File, opts => opts.MapFrom(m => ConvertFormFileToByteArray(m.File)));

        CreateMap<User, ShortUserDto>();
        CreateMap<Picture, PictureDto>()
            .ForMember(dto => dto.Owner, opts => opts.MapFrom(p => p.User));
    }

    private byte[] ConvertFormFileToByteArray(IFormFile file)
    {
        if (file is null)
            return [];

        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}