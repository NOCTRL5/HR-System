using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EmployeeViewer.Models;
using EmployeeViewer.Services;

namespace EmployeeViewer.Pages.Employees
{
    public class IndexModel : PageModel
    {
        private readonly IEmployeeService _employeeService;

        public IndexModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public PagedResult<Employee> PagedEmployees { get; set; } = new PagedResult<Employee>();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            var filter = new EmployeeFilterDto
            {
                SearchTerm = SearchTerm,
                PageNumber = PageNumber,
                PageSize = 5
            };

            PagedEmployees = await _employeeService.GetEmployeesPagedAsync(filter);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                bool deleted = await _employeeService.DeleteEmployeeAsync(id);
                if (deleted)
                {
                    StatusMessage = $"Employee #{id} was successfully deleted.";
                }
                else
                {
                    StatusMessage = $"Error: Could not find employee #{id} to delete.";
                }
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Error deleting employee: {ex.Message}";
            }

            return RedirectToPage("./Index", new { SearchTerm, PageNumber });
        }
    }
}
