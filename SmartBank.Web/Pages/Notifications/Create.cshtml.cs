using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

namespace SmartBank.Web.Pages.Notifications
{
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public class CreateModel : PageModel
    {
        private readonly INotificationApiClient _notificationApiClient;

        public CreateModel(INotificationApiClient notificationApiClient)
        {
            _notificationApiClient = notificationApiClient;
        }

        [BindProperty]
        public CreateNotificationDto Input { get; set; } = new();

        public IEnumerable<string> TypeOptions => NotificationTypes.All;

        public void OnGet(int? customerId)
        {
            if (customerId.HasValue)
            {
                Input.CustomerId = customerId.Value;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _notificationApiClient.CreateAsync(Input);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Failed to send notification.");
                return Page();
            }

            TempData["SuccessMessage"] = "Notification sent successfully.";
            return RedirectToPage("/Notifications/Index", new { customerId = Input.CustomerId });
        }
    }
}
