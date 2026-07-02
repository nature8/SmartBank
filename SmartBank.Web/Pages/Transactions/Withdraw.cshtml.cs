using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

namespace SmartBank.Web.Pages.Transactions
{
    [Authorize]
    public class WithdrawModel : PageModel
    {
        private readonly ITransactionApiClient _transactionApiClient;

        public WithdrawModel(ITransactionApiClient transactionApiClient)
        {
            _transactionApiClient = transactionApiClient;
        }

        [BindProperty]
        public WithdrawDto Input { get; set; } = new();

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

            var result = await _transactionApiClient.WithdrawAsync(Input);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Withdrawal failed.");
                return Page();
            }

            TempData["SuccessMessage"] = $"Withdrawal of {Input.Amount:C} from account {Input.AccountId} was successful.";
            return RedirectToPage("/Transactions/Statement", new { accountId = Input.AccountId });
        }
    }
}
