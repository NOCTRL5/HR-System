using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using EmployeeViewer.Models;

namespace EmployeeViewer.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<PagedResult<Employee>> GetPagedAndFilteredAsync(EmployeeFilterDto filter)
        {
            var result = new PagedResult<Employee>
            {
                PageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber,
                PageSize = filter.PageSize < 1 ? 5 : filter.PageSize
            };

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("dbo.sp_GetEmployeesPagedAndFiltered", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@SearchTerm", (object?)filter.SearchTerm ?? DBNull.Value);
            command.Parameters.AddWithValue("@PageNumber", result.PageNumber);
            command.Parameters.AddWithValue("@PageSize", result.PageSize);

            var totalRecordsParam = new SqlParameter("@TotalRecords", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(totalRecordsParam);

            await connection.OpenAsync();

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    result.Items.Add(MapEmployeeFromReader(reader));
                }
            }

            if (totalRecordsParam.Value != DBNull.Value)
            {
                result.TotalRecords = Convert.ToInt32(totalRecordsParam.Value);
            }

            return result;
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("dbo.sp_GetEmployeeById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@ID", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapEmployeeFromReader(reader);
            }

            return null;
        }

        public async Task<int> CreateAsync(Employee employee)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("dbo.sp_CreateEmployee", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@FirstName", employee.FirstName);
            command.Parameters.AddWithValue("@LastName", employee.LastName);
            command.Parameters.AddWithValue("@Email", employee.Email);
            command.Parameters.AddWithValue("@Phone", (object?)employee.Phone ?? DBNull.Value);
            command.Parameters.AddWithValue("@HireDate", employee.HireDate);
            command.Parameters.AddWithValue("@Salary", employee.Salary);
            command.Parameters.AddWithValue("@ManagerID", (object?)employee.ManagerID ?? DBNull.Value);

            var newIdParam = new SqlParameter("@NewID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newIdParam);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return Convert.ToInt32(newIdParam.Value);
        }

        public async Task<bool> UpdateAsync(Employee employee)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("dbo.sp_UpdateEmployee", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@ID", employee.ID);
            command.Parameters.AddWithValue("@FirstName", employee.FirstName);
            command.Parameters.AddWithValue("@LastName", employee.LastName);
            command.Parameters.AddWithValue("@Email", employee.Email);
            command.Parameters.AddWithValue("@Phone", (object?)employee.Phone ?? DBNull.Value);
            command.Parameters.AddWithValue("@HireDate", employee.HireDate);
            command.Parameters.AddWithValue("@Salary", employee.Salary);
            command.Parameters.AddWithValue("@ManagerID", (object?)employee.ManagerID ?? DBNull.Value);

            await connection.OpenAsync();
            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("dbo.sp_DeleteEmployee", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@ID", id);

            await connection.OpenAsync();
            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<int> GetTotalCountAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            // Execute SQL User-Defined Function (UDF)
            using var command = new SqlCommand("SELECT dbo.fn_GetTotalEmployeesCount()", connection)
            {
                CommandType = CommandType.Text
            };

            await connection.OpenAsync();
            var scalarResult = await command.ExecuteScalarAsync();
            return scalarResult != null && scalarResult != DBNull.Value ? Convert.ToInt32(scalarResult) : 0;
        }

        private static Employee MapEmployeeFromReader(SqlDataReader reader)
        {
            return new Employee
            {
                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                FullName = reader.IsDBNull(reader.GetOrdinal("FullName")) 
                    ? $"{reader.GetString(reader.GetOrdinal("FirstName"))} {reader.GetString(reader.GetOrdinal("LastName"))}"
                    : reader.GetString(reader.GetOrdinal("FullName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? string.Empty : reader.GetString(reader.GetOrdinal("Phone")),
                HireDate = reader.GetDateTime(reader.GetOrdinal("HireDate")),
                Salary = reader.GetDecimal(reader.GetOrdinal("Salary")),
                ManagerID = reader.IsDBNull(reader.GetOrdinal("ManagerID")) ? null : reader.GetInt32(reader.GetOrdinal("ManagerID"))
            };
        }
    }
}
