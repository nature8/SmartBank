using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

namespace SmartBank.Web.Pages.Notifications
{
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public class IndexModel : PageModel
    {
        private readonly INotificationApiClient _notificationApiClient;

        public IndexModel(INotificationApiClient notificationApiClient)
        {
            _notificationApiClient = notificationApiClient;
        }

        public int? CustomerId { get; set; }
        public List<NotificationResponseDto> Notifications { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public bool Searched { get; set; }

        public async Task OnGetAsync(int? customerId)
        {
            CustomerId = customerId;

            if (customerId is null || customerId <= 0)
            {
                return;
            }

            Searched = true;
            var result = await _notificationApiClient.GetByCustomerAsync(customerId.Value);

            if (!result.Success)
            {
                ErrorMessage = result.ErrorMessage;
                return;
            }

            Notifications = result.Data ?? new List<NotificationResponseDto>();
        }
    }
}
