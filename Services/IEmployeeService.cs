using System.Threading.Tasks;
using EmployeeViewer.Models;

namespace EmployeeViewer.Services
{
    public interface IEmployeeService
    {
        Task<PagedResult<Employee>> GetEmployeesPagedAsync(EmployeeFilterDto filter);
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<int> CreateEmployeeAsync(Employee employee);
        Task<bool> UpdateEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<int> GetTotalCountAsync();
    }
}
