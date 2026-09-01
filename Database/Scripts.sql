-- ============================================================================
-- Database Script for EmployeeViewer Application
-- Contains: Table Schema, Seed Data, Functions, and Stored Procedures
-- ============================================================================

-- 1. Create Table (if not exists)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Employee]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Employee](
        [ID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [FirstName] NVARCHAR(50) NOT NULL,
        [LastName] NVARCHAR(50) NOT NULL,
        [Email] NVARCHAR(100) NOT NULL UNIQUE,
        [Phone] NVARCHAR(20) NULL,
        [HireDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [Salary] DECIMAL(8, 3) NOT NULL,
        [ManagerID] INT NULL FOREIGN KEY REFERENCES [dbo].[Employee]([ID])
    );
END
GO

-- 2. User Defined Functions (UDFs)
-- Scalar Function: Format Employee Full Name
IF OBJECT_ID('dbo.fn_FormatEmployeeName', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_FormatEmployeeName;
GO

CREATE FUNCTION dbo.fn_FormatEmployeeName
(
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50)
)
RETURNS NVARCHAR(105)
AS
BEGIN
    RETURN ISNULL(@FirstName, '') + ' ' + ISNULL(@LastName, '');
END;
GO

-- Scalar Function: Get Total Employee Count
IF OBJECT_ID('dbo.fn_GetTotalEmployeesCount', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_GetTotalEmployeesCount;
GO

CREATE FUNCTION dbo.fn_GetTotalEmployeesCount()
RETURNS INT
AS
BEGIN
    DECLARE @TotalCount INT;
    SELECT @TotalCount = COUNT(*) FROM dbo.Employee;
    RETURN ISNULL(@TotalCount, 0);
END;
GO

-- 3. Stored Procedures

-- SP 1: Paged and Filtered Employees Search
IF OBJECT_ID('dbo.sp_GetEmployeesPagedAndFiltered', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetEmployeesPagedAndFiltered;
GO

CREATE PROCEDURE dbo.sp_GetEmployeesPagedAndFiltered
    @SearchTerm NVARCHAR(100) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 5,
    @TotalRecords INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SET @SearchTerm = NULLIF(TRIM(@SearchTerm), '');

    -- Calculate total matching records
    SELECT @TotalRecords = COUNT(1)
    FROM dbo.Employee e
    WHERE @SearchTerm IS NULL OR (
        e.FirstName LIKE '%' + @SearchTerm + '%' OR
        e.LastName LIKE '%' + @SearchTerm + '%' OR
        e.Email LIKE '%' + @SearchTerm + '%' OR
        e.Phone LIKE '%' + @SearchTerm + '%'
    );

    -- Return paginated page
    SELECT 
        e.ID,
        e.FirstName,
        e.LastName,
        dbo.fn_FormatEmployeeName(e.FirstName, e.LastName) AS FullName,
        e.Email,
        e.Phone,
        e.HireDate,
        e.Salary,
        e.ManagerID
    FROM dbo.Employee e
    WHERE @SearchTerm IS NULL OR (
        e.FirstName LIKE '%' + @SearchTerm + '%' OR
        e.LastName LIKE '%' + @SearchTerm + '%' OR
        e.Email LIKE '%' + @SearchTerm + '%' OR
        e.Phone LIKE '%' + @SearchTerm + '%'
    )
    ORDER BY e.ID DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- SP 2: Get Employee By ID
IF OBJECT_ID('dbo.sp_GetEmployeeById', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetEmployeeById;
GO

CREATE PROCEDURE dbo.sp_GetEmployeeById
    @ID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        e.ID,
        e.FirstName,
        e.LastName,
        dbo.fn_FormatEmployeeName(e.FirstName, e.LastName) AS FullName,
        e.Email,
        e.Phone,
        e.HireDate,
        e.Salary,
        e.ManagerID
    FROM dbo.Employee e
    WHERE e.ID = @ID;
END;
GO

-- SP 3: Create Employee
IF OBJECT_ID('dbo.sp_CreateEmployee', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CreateEmployee;
GO

CREATE PROCEDURE dbo.sp_CreateEmployee
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(20),
    @HireDate DATETIME,
    @Salary DECIMAL(8, 3),
    @ManagerID INT = NULL,
    @NewID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Employee (FirstName, LastName, Email, Phone, HireDate, Salary, ManagerID)
    VALUES (@FirstName, @LastName, @Email, @Phone, @HireDate, @Salary, @ManagerID);

    SET @NewID = SCOPE_IDENTITY();
END;
GO

-- SP 4: Update Employee
IF OBJECT_ID('dbo.sp_UpdateEmployee', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_UpdateEmployee;
GO

CREATE PROCEDURE dbo.sp_UpdateEmployee
    @ID INT,
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(20),
    @HireDate DATETIME,
    @Salary DECIMAL(8, 3),
    @ManagerID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Employee
    SET FirstName = @FirstName,
        LastName = @LastName,
        Email = @Email,
        Phone = @Phone,
        HireDate = @HireDate,
        Salary = @Salary,
        ManagerID = @ManagerID
    WHERE ID = @ID;
END;
GO

-- SP 5: Delete Employee
IF OBJECT_ID('dbo.sp_DeleteEmployee', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_DeleteEmployee;
GO

CREATE PROCEDURE dbo.sp_DeleteEmployee
    @ID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Clear manager references to this employee if any exist before deleting
    UPDATE dbo.Employee SET ManagerID = NULL WHERE ManagerID = @ID;

    DELETE FROM dbo.Employee WHERE ID = @ID;
END;
GO

-- 4. Seed Data Script (Run if table is empty)
IF NOT EXISTS (SELECT 1 FROM dbo.Employee)
BEGIN
    INSERT INTO dbo.Employee (FirstName, LastName, Email, Phone, HireDate, Salary, ManagerID)
    VALUES 
    ('John', 'Doe', 'john.doe@example.com', '555-0101', '2022-01-15', 75000.000, NULL),
    ('Jane', 'Smith', 'jane.smith@example.com', '555-0102', '2021-03-20', 82000.500, 1),
    ('Michael', 'Johnson', 'michael.j@example.com', '555-0103', '2023-06-10', 68000.750, 1),
    ('Emily', 'Davis', 'emily.davis@example.com', '555-0104', '2020-11-01', 95000.000, NULL),
    ('Robert', 'Wilson', 'robert.w@example.com', '555-0105', '2022-08-14', 62000.000, 4),
    ('Sarah', 'Brown', 'sarah.b@example.com', '555-0106', '2024-02-01', 58000.250, 4),
    ('David', 'Miller', 'david.m@example.com', '555-0107', '2019-05-18', 105000.000, NULL),
    ('James', 'Taylor', 'james.t@example.com', '555-0108', '2023-09-25', 71000.000, 7);
END
GO
