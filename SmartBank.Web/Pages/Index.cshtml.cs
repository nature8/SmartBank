using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

namespace SmartBank.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICustomerApiClient _customerApiClient;
        private readonly IAccountApiClient _accountApiClient;

        public IndexModel(ICustomerApiClient customerApiClient, IAccountApiClient accountApiClient)
        {
            _customerApiClient = customerApiClient;
            _accountApiClient = accountApiClient;
        }

        public bool IsStaff => User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Manager);

        public int? TotalCustomers { get; set; }
        public CustomerDto? MyCustomerRecord { get; set; }
        public List<AccountResponseDto> MyAccounts { get; set; } = new();
        public string? LoadWarning { get; set; }

        public async Task OnGetAsync()
        {
            if (IsStaff)
            {
                var customers = await _customerApiClient.GetAllAsync();
                if (customers.Success)
                {
                    TotalCustomers = customers.Data?.Count ?? 0;
                }
                else
                {
                    LoadWarning = customers.ErrorMessage;
                }
                return;
            }

            // Customer role: find their own customer record (matched by the Auth UserId claim)
            // and pull their accounts, so the dashboard feels personal without needing a new
            // backend "my accounts" endpoint.
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return;
            }

            var allCustomers = await _customerApiClient.GetAllAsync();
            if (!allCustomers.Success)
            {
                LoadWarning = allCustomers.ErrorMessage;
                return;
            }

            MyCustomerRecord = allCustomers.Data?.FirstOrDefault(c => c.UserId == userId);

            if (MyCustomerRecord != null)
            {
                var accounts = await _accountApiClient.GetByCustomerAsync(MyCustomerRecord.CustomerId);
                if (accounts.Success)
                {
                    MyAccounts = accounts.Data ?? new List<AccountResponseDto>();
                }
                else
                {
                    LoadWarning = accounts.ErrorMessage;
                }
            }
        }
    }
}
