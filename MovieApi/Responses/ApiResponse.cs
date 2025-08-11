namespace MovieApi.Responses;

public class ApiResponse<T>
{
    public bool Success { get; init; } = true;
    public T? Data { get; init; }
    public object? Meta { get; init; }

    public static ApiResponse<T> Ok(T data, object? meta = null)
        => new() { Success = true, Data = data, Meta = meta };
}
