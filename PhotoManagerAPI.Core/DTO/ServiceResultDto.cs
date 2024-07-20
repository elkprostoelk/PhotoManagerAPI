using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PhotoManagerAPI.Core.DTO;

public class ServiceResultDto
{
    public bool IsSuccess { get; set; } = true;

    public List<string> Errors { get; set; } = [];

    public void AddToModelState(ModelStateDictionary modelState)
    {
        if (Errors.Count > 0)
        {
            Errors.ForEach(e => modelState.AddModelError(string.Empty, e));
        }
    }
}

public class ServiceResultDto<T> : ServiceResultDto where T: new()
{
    public T? Container { get; set; }
}