using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

namespace SmartBank.Web.Pages.Accounts
{
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public class IndexModel : PageModel
    {
        private readonly IAccountApiClient _accountApiClient;

        public IndexModel(IAccountApiClient accountApiClient)
        {
            _accountApiClient = accountApiClient;
        }

        public int? CustomerId { get; set; }
        public List<AccountResponseDto> Accounts { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public bool Searched { get; set; }

        public async Task OnGetAsync(int? customerId)
        {
            CustomerId = customerId;

            if (customerId is null || customerId <= 0)
            {
                return;
            }

            Searched = true;
            var result = await _accountApiClient.GetByCustomerAsync(customerId.Value);

            if (!result.Success)
            {
                ErrorMessage = result.ErrorMessage;
                return;
            }

            Accounts = result.Data ?? new List<AccountResponseDto>();
        }
    }
}
