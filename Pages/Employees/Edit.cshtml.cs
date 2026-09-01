using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EmployeeViewer.Models;
using EmployeeViewer.Services;

namespace EmployeeViewer.Pages.Employees
{
    public class EditModel : PageModel
    {
        private readonly IEmployeeService _employeeService;

        public EditModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [BindProperty]
        public Employee Employee { get; set; } = new Employee();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var emp = await _employeeService.GetEmployeeByIdAsync(id);
            if (emp == null)
            {
                StatusMessage = $"Error: Employee with ID #{id} was not found.";
                return RedirectToPage("./Index");
            }

            Employee = emp;
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
                bool updated = await _employeeService.UpdateEmployeeAsync(Employee);
                if (updated)
                {
                    StatusMessage = $"Employee #{Employee.ID} ('{Employee.FirstName} {Employee.LastName}') updated successfully.";
                    return RedirectToPage("./Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Update failed. Employee record not found.");
                    return Page();
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating employee: {ex.Message}");
                return Page();
            }
        }
    }
}
