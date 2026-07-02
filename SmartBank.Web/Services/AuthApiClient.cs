using System.Net.Http.Json;
using SmartBank.Web.Models;

namespace SmartBank.Web.Services
{
    public interface IAuthApiClient
    {
        Task<ApiResult<LoginResponseDto>> LoginAsync(LoginRequestDto request);
        Task<ApiResult<string>> RegisterAsync(RegisterRequestDto request);
        Task<ApiResult<ProfileDto>> GetProfileAsync();
    }

    /// <summary>
    /// Talks to AuthService through the Gateway route "/auth/**" which forwards to
    /// AuthController's "api/Auth/**" routes.
    /// </summary>
    public class AuthApiClient : IAuthApiClient
    {
        private readonly HttpClient _http;

        public AuthApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResult<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("auth/api/Auth/login", request);

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResult<LoginResponseDto>.Fail(await response.ReadErrorMessageAsync());
                }

                var data = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                return data is null
                    ? ApiResult<LoginResponseDto>.Fail("Unexpected empty response from the authentication service.")
                    : ApiResult<LoginResponseDto>.Ok(data);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<LoginResponseDto>.Fail($"Could not reach the Auth service through the Gateway: {ex.Message}");
            }
        }

        public async Task<ApiResult<string>> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("auth/api/Auth/register", request);

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResult<string>.Fail(await response.ReadErrorMessageAsync());
                }

                var payload = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                var message = payload != null && payload.TryGetValue("message", out var m) ? m : "Registration successful.";

                // AuthService returns 200 OK even for "Email already exists." - surface that as a failure.
                if (message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                {
                    return ApiResult<string>.Fail(message);
                }

                return ApiResult<string>.Ok(message);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<string>.Fail($"Could not reach the Auth service through the Gateway: {ex.Message}");
            }
        }

        public async Task<ApiResult<ProfileDto>> GetProfileAsync()
        {
            try
            {
                var response = await _http.GetAsync("auth/api/Auth/profile");

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResult<ProfileDto>.Fail(await response.ReadErrorMessageAsync());
                }

                var data = await response.Content.ReadFromJsonAsync<ProfileDto>();
                return data is null
                    ? ApiResult<ProfileDto>.Fail("Unexpected empty response from the authentication service.")
                    : ApiResult<ProfileDto>.Ok(data);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<ProfileDto>.Fail($"Could not reach the Auth service through the Gateway: {ex.Message}");
            }
        }
    }
}
