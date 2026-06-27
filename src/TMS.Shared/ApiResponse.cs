using System.Collections.Generic;

namespace TMS.Shared;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public int StatusCode { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null) => new ApiResponse<T>
    {
        Success    = true,
        Data       = data,
        Message    = message,
        StatusCode = 200
    };

    public static ApiResponse<T> Fail(string message, int statusCode) => new ApiResponse<T>
    {
        Success    = false,
        Message    = message,
        StatusCode = statusCode
    };

    public static ApiResponse<T> Fail(List<string> errors, int statusCode) => new ApiResponse<T>
    {
        Success    = false,
        Errors     = errors,
        StatusCode = statusCode
    };
}
