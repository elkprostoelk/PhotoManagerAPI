using AutoMapper;
using PhotoManagerAPI.Core.DTO;
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