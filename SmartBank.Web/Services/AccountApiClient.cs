using System.Net.Http.Json;
using SmartBank.Web.Models;

namespace SmartBank.Web.Services
{
    public interface IAccountApiClient
    {
        Task<ApiResult<AccountResponseDto>> CreateAsync(CreateAccountDto dto);
        Task<ApiResult<AccountResponseDto>> GetAsync(int accountId);
        Task<ApiResult<List<AccountResponseDto>>> GetByCustomerAsync(int customerId);
        Task<ApiResult> CloseAsync(int accountId);
    }

    /// <summary>
    /// Talks to AccountService through the Gateway route "/account/**" which forwards to
    /// AccountController's "api/Account/**" routes.
    /// </summary>
    public class AccountApiClient : IAccountApiClient
    {
        private readonly HttpClient _http;

        public AccountApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResult<AccountResponseDto>> CreateAsync(CreateAccountDto dto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("account/api/Account", dto);

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResult<AccountResponseDto>.Fail(await response.ReadErrorMessageAsync());
                }

                var data = await response.Content.ReadFromJsonAsync<AccountResponseDto>();
                return data is null
                    ? ApiResult<AccountResponseDto>.Fail("Unexpected empty response from the Account service.")
                    : ApiResult<AccountResponseDto>.Ok(data);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<AccountResponseDto>.Fail($"Could not reach the Account service through the Gateway: {ex.Message}");
            }
        }

        public async Task<ApiResult<AccountResponseDto>> GetAsync(int accountId)
        {
            try
            {
                var response = await _http.GetAsync($"account/api/Account/{accountId}");

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResult<AccountResponseDto>.Fail(await response.ReadErrorMessageAsync());
                }

                var data = await response.Content.ReadFromJsonAsync<AccountResponseDto>();
                return data is null
                    ? ApiResult<AccountResponseDto>.Fail("Account not found.")
                    : ApiResult<AccountResponseDto>.Ok(data);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<AccountResponseDto>.Fail($"Could not reach the Account service through the Gateway: {ex.Message}");
            }
        }

        public async Task<ApiResult<List<AccountResponseDto>>> GetByCustomerAsync(int customerId)
        {
            try
            {
                var response = await _http.GetAsync($"account/api/Account/customer/{customerId}");

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResult<List<AccountResponseDto>>.Fail(await response.ReadErrorMessageAsync());
                }

                var data = await response.Content.ReadFromJsonAsync<List<AccountResponseDto>>();
                return ApiResult<List<AccountResponseDto>>.Ok(data ?? new List<AccountResponseDto>());
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<List<AccountResponseDto>>.Fail($"Could not reach the Account service through the Gateway: {ex.Message}");
            }
        }

        public async Task<ApiResult> CloseAsync(int accountId)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("account/api/Account/close", new CloseAccountDto { AccountId = accountId });

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResult.Fail(await response.ReadErrorMessageAsync());
                }

                return ApiResult.Ok();
            }
            catch (HttpRequestException ex)
            {
                return ApiResult.Fail($"Could not reach the Account service through the Gateway: {ex.Message}");
            }
        }
    }
}
