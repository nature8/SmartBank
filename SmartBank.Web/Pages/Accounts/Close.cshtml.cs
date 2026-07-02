using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

namespace SmartBank.Web.Pages.Accounts
{
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public class CloseModel : PageModel
    {
        private readonly IAccountApiClient _accountApiClient;

        public CloseModel(IAccountApiClient accountApiClient)
        {
            _accountApiClient = accountApiClient;
        }

        public AccountResponseDto? Account { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var result = await _accountApiClient.GetAsync(id);

            if (!result.Success || result.Data is null)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Account not found.";
                return RedirectToPage("/Accounts/Index");
            }

            Account = result.Data;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var result = await _accountApiClient.CloseAsync(id);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to close the account.";
                return RedirectToPage("/Accounts/Index");
            }

            TempData["SuccessMessage"] = "Account closed successfully.";
            return RedirectToPage("/Accounts/Index");
        }
    }
}
