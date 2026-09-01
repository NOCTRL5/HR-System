using System;
using System.Threading.Tasks;
using EmployeeViewer.Models;
using EmployeeViewer.Repositories;

namespace EmployeeViewer.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public Task<PagedResult<Employee>> GetEmployeesPagedAsync(EmployeeFilterDto filter)
        {
            return _repository.GetPagedAndFilteredAsync(filter);
        }

        public Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }

        public Task<int> CreateEmployeeAsync(Employee employee)
        {
            if (string.IsNullOrWhiteSpace(employee.FirstName))
                throw new ArgumentException("First Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(employee.LastName))
                throw new ArgumentException("Last Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(employee.Email))
                throw new ArgumentException("Email cannot be empty.");

            return _repository.CreateAsync(employee);
        }

        public Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            if (employee.ID <= 0)
                throw new ArgumentException("Invalid Employee ID.");

            return _repository.UpdateAsync(employee);
        }

        public Task<bool> DeleteEmployeeAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Employee ID.");

            return _repository.DeleteAsync(id);
        }

        public Task<int> GetTotalCountAsync()
        {
            return _repository.GetTotalCountAsync();
        }
    }
}
