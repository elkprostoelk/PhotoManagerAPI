using Microsoft.AspNetCore.Mvc.ModelBinding;
using PhotoManagerAPI.Core.DTO;

namespace PhotoManagerAPI.Web.Extensions;

public class ModelStateExtensions
{
    public static void AddErrors(ModelStateDictionary modelState, ServiceResultDto dto)
    {
        if (dto.Errors.Count > 0)
        {
            dto.Errors.ForEach(e => modelState.AddModelError("errors", e));
        }
    }
}