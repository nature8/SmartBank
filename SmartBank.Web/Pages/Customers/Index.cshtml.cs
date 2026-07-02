using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

namespace SmartBank.Web.Pages.Customers
{
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public class IndexModel : PageModel
    {
        private readonly ICustomerApiClient _customerApiClient;

        public IndexModel(ICustomerApiClient customerApiClient)
        {
            _customerApiClient = customerApiClient;
        }

        public List<CustomerDto> Customers { get; set; } = new();
        public string? SearchTerm { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync(string? search)
        {
            SearchTerm = search;

            var result = await _customerApiClient.GetAllAsync();

            if (!result.Success)
            {
                ErrorMessage = result.ErrorMessage;
                return;
            }

            Customers = result.Data ?? new List<CustomerDto>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                Customers = Customers.Where(c =>
                        c.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        c.LastName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        c.Email.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        c.CustomerId.ToString() == term)
                    .ToList();
            }
        }
    }
}
