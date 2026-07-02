using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

namespace SmartBank.Web.Pages.Customers
{
    [Authorize(Roles = Roles.Customer)]
    public class MyProfileModel : PageModel
    {
        private readonly ICustomerApiClient _customerApiClient;

        public MyProfileModel(ICustomerApiClient customerApiClient)
        {
            _customerApiClient = customerApiClient;
        }

        [BindProperty]
        public CustomerDto Input { get; set; } = new();

        public bool HasProfile { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            var userId = GetCurrentUserId();
            if (userId is null) return;

            // No "get customer by userId" endpoint exists on CustomerService, so we fetch the
            // full list and match by the linked Auth UserId - the same approach as the dashboard.
            var all = await _customerApiClient.GetAllAsync();
            if (!all.Success)
            {
                ErrorMessage = all.ErrorMessage;
                return;
            }

            var mine = all.Data?.FirstOrDefault(c => c.UserId == userId);
            if (mine != null)
            {
                Input = mine;
                HasProfile = true;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Forbid();
            }

            // A customer may only ever edit their own record - force the linked UserId
            // regardless of anything posted from the form.
            Input.UserId = userId.Value;

            if (!ModelState.IsValid)
            {
                HasProfile = Input.CustomerId > 0;
                return Page();
            }

            var result = await _customerApiClient.UpdateAsync(Input.CustomerId, Input);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Failed to update your profile.");
                HasProfile = true;
                return Page();
            }

            TempData["SuccessMessage"] = "Your profile was updated successfully.";
            return RedirectToPage("/Customers/MyProfile");
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}
