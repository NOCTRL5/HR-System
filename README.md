# HR System (Employee Viewer)

A high-performance, scalable Employee Management Web Application built with **ASP.NET Core Razor Pages**, **ADO.NET** (`Microsoft.Data.SqlClient`), **SQL Server Stored Procedures**, and **User-Defined Functions (UDFs)**.

---

## Features

- **Decoupled Layered Architecture**: Built with Repository & Service patterns for scalability and AI Agent readiness.
- **SQL Stored Procedures & Functions**:
  - `sp_GetEmployeesPagedAndFiltered`: Server-side search filtering and pagination.
  - `sp_CreateEmployee`, `sp_UpdateEmployee`, `sp_DeleteEmployee`, `sp_GetEmployeeById`: Complete CRUD stored procedures.
  - `fn_FormatEmployeeName`, `fn_GetTotalEmployeesCount`: SQL User-Defined Functions.
- **Modern Responsive UI**:
  - Welcome Landing Page with system metrics and feature cards.
  - Employee Directory with live search, pagination, and delete confirmation modals.
  - Interactive Create, Edit, and Detail view forms.

---

## Getting Started

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/)
- [SQL Server](https://www.microsoft.com/sql-server/)

### Database Setup
1. Open SQL Server Management Studio (SSMS) or `sqlcmd`.
2. Execute the database script located at [`Database/Scripts.sql`](Database/Scripts.sql) against your SQL database (e.g. `HR`).

### Configuration
Verify connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HR;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Running the Web Server
Run the project using terminal:
```bash
dotnet run
```
Open your browser at `http://localhost:5025` (Welcome Page) or `http://localhost:5025/Employees` (Employee Directory).