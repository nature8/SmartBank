using System.Net.Http.Json;
using SmartBank.Web.Models;

namespace SmartBank.Web.Services
{
    public interface ICustomerApiClient
    {
        Task<ApiResult<List<CustomerDto>>> GetAllAsync();
        Task<ApiResult<CustomerDto>> GetByIdAsync(int id);
        Task<ApiResult<CustomerDto>> CreateAsync(CustomerDto customer);
        Task<ApiResult> UpdateAsync(int id, CustomerDto customer);
        Task<ApiResult> DeleteAsync(int id);
    }

    /// <summary>
    /// Talks to CustomerService through the Gateway route "/customer/**" which forwards to
    /// CustomerController's "api/Customer/**" routes.
    /// </summary>
    public class CustomerApiClient : ICustomerApiClient
    {
        private readonly HttpClient _http;

        public CustomerApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResult<List<CustomerDto>>> GetAllAsync()
        {
            try
            {
                var response = await _http.GetAsync("customer/api/Customer");

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResult<List<CustomerDto>>.Fail(await response.ReadErrorMessageAsync());
                }

                var data = await response.Content.ReadFromJsonAsync<List<CustomerDto>>();
                return ApiResult<List<CustomerDto>>.Ok(data ?? new List<CustomerDto>());
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<List<CustomerDto>>.Fail($"Could not reach the Customer service through the Gateway: {ex.Message}");
            }
        }

        public async Task<ApiResult<CustomerDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _http.GetAsync($"customer/api/Customer/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResult<CustomerDto>.Fail(await response.ReadErrorMessageAsync());
                }

                var data = await response.Content.ReadFromJsonAsync<CustomerDto>();
                return data is null
                    ? ApiResult<CustomerDto>.Fail("Customer not found.")
                    : ApiResult<CustomerDto>.Ok(data);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<CustomerDto>.Fail($"Could not reach the Customer service through the Gateway: {ex.Message}");
            }
        }

        public async Task<ApiResult<CustomerDto>> CreateAsync(CustomerDto customer)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("customer/api/Customer", customer);

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResult<CustomerDto>.Fail(await response.ReadErrorMessageAsync());
                }

                var data = await response.Content.ReadFromJsonAsync<CustomerDto>();
                return data is null
                    ? ApiResult<CustomerDto>.Fail("Unexpected empty response from the Customer service.")
                    : ApiResult<CustomerDto>.Ok(data);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<CustomerDto>.Fail($"Could not reach the Customer service through the Gateway: {ex.Message}");
            }
        }

        public async Task<ApiResult> UpdateAsync(int id, CustomerDto customer)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"customer/api/Customer/{id}", customer);

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResult.Fail(await response.ReadErrorMessageAsync());
                }

                return ApiResult.Ok();
            }
            catch (HttpRequestException ex)
            {
                return ApiResult.Fail($"Could not reach the Customer service through the Gateway: {ex.Message}");
            }
        }

        public async Task<ApiResult> DeleteAsync(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"customer/api/Customer/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResult.Fail(await response.ReadErrorMessageAsync());
                }

                return ApiResult.Ok();
            }
            catch (HttpRequestException ex)
            {
                return ApiResult.Fail($"Could not reach the Customer service through the Gateway: {ex.Message}");
            }
        }
    }
}
