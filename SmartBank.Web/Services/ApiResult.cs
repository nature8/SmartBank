namespace SmartBank.Web.Services
{
    /// <summary>Uniform wrapper so Razor Page handlers don't need to deal with HttpResponseMessage directly.</summary>
    public class ApiResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static ApiResult Ok() => new() { Success = true };
        public static ApiResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }

    public class ApiResult<T> : ApiResult
    {
        public T? Data { get; set; }

        public static ApiResult<T> Ok(T data) => new() { Success = true, Data = data };
        public static new ApiResult<T> Fail(string message) => new() { Success = false, ErrorMessage = message };
    }
}
