namespace PhotoManagerAPI.Core.DTO;

public class ServiceResultDto
{
    public bool IsSuccess { get; set; } = true;

    public List<string> Errors { get; set; } = [];
}

public class ServiceResultDto<T> : ServiceResultDto where T: new()
{
    public T? Container { get; set; }
}