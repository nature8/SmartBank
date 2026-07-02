using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

namespace SmartBank.Web.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly IAuthApiClient _authApiClient;

        public RegisterModel(IAuthApiClient authApiClient)
        {
            _authApiClient = authApiClient;
        }

        [BindProperty]
        public RegisterRequestDto Input { get; set; } = new() { RoleId = Roles.CustomerId };

        /// <summary>Only Admins (already signed in) may register staff members with a non-Customer role.</summary>
        public bool CanChooseRole => User.Identity?.IsAuthenticated == true && User.IsInRole(Roles.Admin);

        public IEnumerable<(int Id, string Name)> RoleOptions => Roles.All;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Public self-registration can never grant Admin/Manager - force Customer
            // unless the currently signed-in user is an Admin creating a staff account.
            if (!CanChooseRole)
            {
                Input.RoleId = Roles.CustomerId;
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _authApiClient.RegisterAsync(Input);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Registration failed.");
                return Page();
            }

            TempData["SuccessMessage"] = "Account created successfully. Please sign in.";
            return RedirectToPage("/Account/Login");
        }
    }
}
