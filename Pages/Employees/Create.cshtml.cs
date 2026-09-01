using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EmployeeViewer.Models;
using EmployeeViewer.Services;

namespace EmployeeViewer.Pages.Employees
{
    public class CreateModel : PageModel
    {
        private readonly IEmployeeService _employeeService;

        public CreateModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [BindProperty]
        public Employee Employee { get; set; } = new Employee();

        [TempData]
        public string? StatusMessage { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                int newId = await _employeeService.CreateEmployeeAsync(Employee);
                StatusMessage = $"Employee '{Employee.FirstName} {Employee.LastName}' was successfully created with ID #{newId}.";
                return RedirectToPage("./Index");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Failed to create employee: {ex.Message}");
                return Page();
            }
        }
    }
}
