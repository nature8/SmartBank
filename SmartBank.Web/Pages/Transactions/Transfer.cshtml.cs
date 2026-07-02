using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

namespace SmartBank.Web.Pages.Transactions
{
    [Authorize]
    public class TransferModel : PageModel
    {
        private readonly ITransactionApiClient _transactionApiClient;

        public TransferModel(ITransactionApiClient transactionApiClient)
        {
            _transactionApiClient = transactionApiClient;
        }

        [BindProperty]
        public TransferDto Input { get; set; } = new();

        public void OnGet(int? fromAccountId)
        {
            if (fromAccountId.HasValue)
            {
                Input.FromAccountId = fromAccountId.Value;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Input.FromAccountId == Input.ToAccountId && Input.FromAccountId != 0)
            {
                ModelState.AddModelError(string.Empty, "The source and destination accounts must be different.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _transactionApiClient.TransferAsync(Input);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Transfer failed.");
                return Page();
            }

            TempData["SuccessMessage"] = $"Transfer of {Input.Amount:C} from account {Input.FromAccountId} to account {Input.ToAccountId} was successful.";
            return RedirectToPage("/Transactions/Statement", new { accountId = Input.FromAccountId });
        }
    }
}
