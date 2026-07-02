using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

namespace SmartBank.Web.Pages.Customers
{
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public class CreateModel : PageModel
    {
        private readonly ICustomerApiClient _customerApiClient;

        public CreateModel(ICustomerApiClient customerApiClient)
        {
            _customerApiClient = customerApiClient;
        }

        [BindProperty]
        public CustomerDto Input { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _customerApiClient.CreateAsync(Input);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Failed to create customer.");
                return Page();
            }

            TempData["SuccessMessage"] = $"Customer '{Input.FirstName} {Input.LastName}' was created successfully.";
            return RedirectToPage("/Customers/Index");
        }
    }
}
