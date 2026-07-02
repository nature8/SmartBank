using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

namespace SmartBank.Web.Pages.Customers
{
    [Authorize(Roles = Roles.Admin)] // Deleting customer records is Admin-only.
    public class DeleteModel : PageModel
    {
        private readonly ICustomerApiClient _customerApiClient;

        public DeleteModel(ICustomerApiClient customerApiClient)
        {
            _customerApiClient = customerApiClient;
        }

        [BindProperty]
        public CustomerDto Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var result = await _customerApiClient.GetByIdAsync(id);

            if (!result.Success || result.Data is null)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Customer not found.";
                return RedirectToPage("/Customers/Index");
            }

            Input = result.Data;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var result = await _customerApiClient.DeleteAsync(id);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to delete customer.";
                return RedirectToPage("/Customers/Index");
            }

            TempData["SuccessMessage"] = "Customer deleted successfully.";
            return RedirectToPage("/Customers/Index");
        }
    }
}
