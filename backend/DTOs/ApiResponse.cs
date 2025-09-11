namespace MyCms.Api.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    
    public static ApiResponse<T> SuccessResult(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }
    
    public static ApiResponse<T> ErrorResult(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message
        };
    }
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    
    public static ApiResponse SuccessResult(string message = "Success")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }
    
    public static ApiResponse ErrorResult(string message)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message
        };
    }
}