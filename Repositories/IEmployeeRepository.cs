using System.Threading.Tasks;
using EmployeeViewer.Models;

namespace EmployeeViewer.Repositories
{
    public interface IEmployeeRepository
    {
        Task<PagedResult<Employee>> GetPagedAndFilteredAsync(EmployeeFilterDto filter);
        Task<Employee?> GetByIdAsync(int id);
        Task<int> CreateAsync(Employee employee);
        Task<bool> UpdateAsync(Employee employee);
        Task<bool> DeleteAsync(int id);
        Task<int> GetTotalCountAsync();
    }
}
