namespace Airbnb.ServiceDefaults.Infrastructure;

public record ApiResponse<T>(
    T? Data, 
    string? Message = null, 
    bool Success = true, 
    List<string>? Errors = null)
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResult(T data, string? message = null) 
        => new(data, message, true);

    public static ApiResponse<T> FailureResult(List<string> errors, string? message = null) 
        => new(default, message, false, errors);
}
