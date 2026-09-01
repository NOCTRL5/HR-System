using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EmployeeViewer.Models;
using EmployeeViewer.Services;

namespace EmployeeViewer.Pages.Employees
{
    public class DetailsModel : PageModel
    {
        private readonly IEmployeeService _employeeService;

        public DetailsModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public Employee Employee { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var emp = await _employeeService.GetEmployeeByIdAsync(id);
            if (emp == null)
            {
                return NotFound();
            }

            Employee = emp;
            return Page();
        }
    }
}
