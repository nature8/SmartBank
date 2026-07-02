using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

namespace SmartBank.Web.Pages.Notifications
{
    [Authorize(Roles = Roles.Customer)]
    public class MyNotificationsModel : PageModel
    {
        private readonly ICustomerApiClient _customerApiClient;
        private readonly INotificationApiClient _notificationApiClient;

        public MyNotificationsModel(ICustomerApiClient customerApiClient, INotificationApiClient notificationApiClient)
        {
            _customerApiClient = customerApiClient;
            _notificationApiClient = notificationApiClient;
        }

        public List<NotificationResponseDto> Notifications { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public bool HasCustomerRecord { get; set; }

        public async Task OnGetAsync()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out var userId))
            {
                return;
            }

            var customers = await _customerApiClient.GetAllAsync();
            if (!customers.Success)
            {
                ErrorMessage = customers.ErrorMessage;
                return;
            }

            var mine = customers.Data?.FirstOrDefault(c => c.UserId == userId);
            if (mine is null)
            {
                return;
            }

            HasCustomerRecord = true;

            var result = await _notificationApiClient.GetByCustomerAsync(mine.CustomerId);
            if (!result.Success)
            {
                ErrorMessage = result.ErrorMessage;
                return;
            }

            Notifications = result.Data ?? new List<NotificationResponseDto>();
        }
    }
}
