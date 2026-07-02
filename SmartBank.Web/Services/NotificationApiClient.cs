using System.Net.Http.Json;
using SmartBank.Web.Models;

namespace SmartBank.Web.Services
{
    public interface INotificationApiClient
    {
        Task<ApiResult<NotificationResponseDto>> CreateAsync(CreateNotificationDto dto);
        Task<ApiResult<List<NotificationResponseDto>>> GetByCustomerAsync(int customerId);
    }

    /// <summary>
    /// Talks to NotificationService through the Gateway route "/notification/**" which forwards to
    /// NotificationController's "api/notification/**" routes.
    /// </summary>
    public class NotificationApiClient : INotificationApiClient
    {
        private readonly HttpClient _http;

        public NotificationApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResult<NotificationResponseDto>> CreateAsync(CreateNotificationDto dto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("notification/api/notification", dto);

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResult<NotificationResponseDto>.Fail(await response.ReadErrorMessageAsync());
                }

                var data = await response.Content.ReadFromJsonAsync<NotificationResponseDto>();
                return data is null
                    ? ApiResult<NotificationResponseDto>.Fail("Unexpected empty response from the Notification service.")
                    : ApiResult<NotificationResponseDto>.Ok(data);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<NotificationResponseDto>.Fail($"Could not reach the Notification service through the Gateway: {ex.Message}");
            }
        }

        public async Task<ApiResult<List<NotificationResponseDto>>> GetByCustomerAsync(int customerId)
        {
            try
            {
                var response = await _http.GetAsync($"notification/api/notification/{customerId}");

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResult<List<NotificationResponseDto>>.Fail(await response.ReadErrorMessageAsync());
                }

                var data = await response.Content.ReadFromJsonAsync<List<NotificationResponseDto>>();
                return ApiResult<List<NotificationResponseDto>>.Ok(data ?? new List<NotificationResponseDto>());
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<List<NotificationResponseDto>>.Fail($"Could not reach the Notification service through the Gateway: {ex.Message}");
            }
        }
    }
}
