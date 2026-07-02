using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

namespace SmartBank.Web.Pages.Transactions
{
    // Any signed-in role can deposit: staff performing teller operations, or a customer
    // depositing into one of their own accounts.
    [Authorize]
    public class DepositModel : PageModel
    {
        private readonly ITransactionApiClient _transactionApiClient;

        public DepositModel(ITransactionApiClient transactionApiClient)
        {
            _transactionApiClient = transactionApiClient;
        }

        [BindProperty]
        public DepositDto Input { get; set; } = new();

        public void OnGet(int? accountId)
        {
            if (accountId.HasValue)
            {
                Input.AccountId = accountId.Value;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _transactionApiClient.DepositAsync(Input);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Deposit failed.");
                return Page();
            }

            TempData["SuccessMessage"] = $"Deposit of {Input.Amount:C} to account {Input.AccountId} was successful.";
            return RedirectToPage("/Transactions/Statement", new { accountId = Input.AccountId });
        }
    }
}
