namespace Airbnb.ServiceDefaults.Infrastructure;

public record ApiResponse<T>(
    T? Data, 
    string? Message = null, 
    bool Success = true, 
    string? ErrorCode = null, 
    List<string>? Errors = null)
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResult(T data, string? message = null) 
        => new(data, message, true);

    public static ApiResponse<T> FailureResult(string errorCode, string? message = null, List<string>? errors = null) 
        => new(default, message, false, errorCode, errors);
}
