using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

namespace SmartBank.Web.Pages.Customers
{
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public class EditModel : PageModel
    {
        private readonly ICustomerApiClient _customerApiClient;

        public EditModel(ICustomerApiClient customerApiClient)
        {
            _customerApiClient = customerApiClient;
        }

        [BindProperty]
        public CustomerDto Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _customerApiClient.UpdateAsync(id, Input);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Failed to update customer.");
                return Page();
            }

            TempData["SuccessMessage"] = "Customer updated successfully.";
            return RedirectToPage("/Customers/Index");
        }
    }
}
