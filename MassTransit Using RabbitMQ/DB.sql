CREATE DATABASE OrderDb;
GO

USE OrderDb;
GO

CREATE TABLE Orders
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(200),
    Quantity INT,
    Price DECIMAL(18,2),
    [Status] NVARCHAR(50),
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO

select * from Orders;