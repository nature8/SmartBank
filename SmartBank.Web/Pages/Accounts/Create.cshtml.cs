using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

namespace SmartBank.Web.Pages.Accounts
{
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public class CreateModel : PageModel
    {
        private readonly IAccountApiClient _accountApiClient;

        public CreateModel(IAccountApiClient accountApiClient)
        {
            _accountApiClient = accountApiClient;
        }

        [BindProperty]
        public CreateAccountDto Input { get; set; } = new();

        public IEnumerable<string> AccountTypeOptions => AccountTypes.All;

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

            var result = await _accountApiClient.CreateAsync(Input);

            if (!result.Success || result.Data is null)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Failed to open account.");
                return Page();
            }

            TempData["SuccessMessage"] = $"Account {result.Data.AccountNumber} opened successfully with balance {result.Data.Balance:C}.";
            return RedirectToPage("/Accounts/Index", new { customerId = Input.CustomerId });
        }
    }
}
