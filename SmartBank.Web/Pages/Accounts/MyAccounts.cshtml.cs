using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

namespace SmartBank.Web.Pages.Accounts
{
    [Authorize(Roles = Roles.Customer)]
    public class MyAccountsModel : PageModel
    {
        private readonly ICustomerApiClient _customerApiClient;
        private readonly IAccountApiClient _accountApiClient;

        public MyAccountsModel(ICustomerApiClient customerApiClient, IAccountApiClient accountApiClient)
        {
            _customerApiClient = customerApiClient;
            _accountApiClient = accountApiClient;
        }

        public List<AccountResponseDto> Accounts { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public bool HasCustomerRecord { get; set; }

        public async Task OnGetAsync()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out var userId))
            {
                return;
            }

            var customers = await _customerApiClient.GetAllAsync();
            if (!customers.Success)
            {
                ErrorMessage = customers.ErrorMessage;
                return;
            }

            var mine = customers.Data?.FirstOrDefault(c => c.UserId == userId);
            if (mine is null)
            {
                return;
            }

            HasCustomerRecord = true;

            var accounts = await _accountApiClient.GetByCustomerAsync(mine.CustomerId);
            if (!accounts.Success)
            {
                ErrorMessage = accounts.ErrorMessage;
                return;
            }

            Accounts = accounts.Data ?? new List<AccountResponseDto>();
        }
    }
}
