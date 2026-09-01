using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EmployeeViewer.Services;

namespace EmployeeViewer.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IEmployeeService _employeeService;

        public IndexModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public int TotalEmployees { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                TotalEmployees = await _employeeService.GetTotalCountAsync();
            }
            catch
            {
                TotalEmployees = 0;
            }
        }
    }
}