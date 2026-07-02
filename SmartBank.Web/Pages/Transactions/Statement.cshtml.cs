using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

namespace SmartBank.Web.Pages.Transactions
{
    [Authorize]
    public class StatementModel : PageModel
    {
        private readonly ITransactionApiClient _transactionApiClient;

        public StatementModel(ITransactionApiClient transactionApiClient)
        {
            _transactionApiClient = transactionApiClient;
        }

        public int? AccountId { get; set; }
        public List<TransactionDto> Transactions { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public bool Searched { get; set; }

        public async Task OnGetAsync(int? accountId)
        {
            AccountId = accountId;

            if (accountId is null || accountId <= 0)
            {
                return;
            }

            Searched = true;
            var result = await _transactionApiClient.GetStatementAsync(accountId.Value);

            if (!result.Success)
            {
                ErrorMessage = result.ErrorMessage;
                return;
            }

            Transactions = result.Data ?? new List<TransactionDto>();
        }
    }
}
