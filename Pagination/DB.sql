
CREATE DATABASE PaginationDemo;
GO

USE PaginationDemo;
GO

SELECT * FROM Employees;
CREATE TABLE Employees
(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100),
    Email NVARCHAR(100),
    Salary INT,
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO

CREATE INDEX IX_Employees_Id ON Employees(Id);
GO

SET STATISTICS TIME ON;
GO

SET NOCOUNT ON;
GO

WITH Numbers AS
(
    SELECT TOP 1000000
        ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS Number
    FROM sys.objects a
    CROSS JOIN sys.objects b
    CROSS JOIN sys.objects c
)
INSERT INTO Employees(Name, Email, Salary)
SELECT
    'Employee_' + CAST(Number AS VARCHAR(20)),
    'employee' + CAST(Number AS VARCHAR(20)) + '@gmail.com',
    ABS(CHECKSUM(NEWID())) % 100000
FROM Numbers;
GO

SELECT * FROM Employees;

SELECT *
FROM Employees
ORDER BY Id
OFFSET 0 ROWS
FETCH NEXT 10 ROWS ONLY;

SELECT *
FROM Employees
ORDER BY Id
OFFSET 900000 ROWS
FETCH NEXT 10 ROWS ONLY;

SELECT TOP 10 * from Employees WHERE Id > 900000;