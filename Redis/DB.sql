
CREATE DATABASE RedisDemoDb;
GO

USE RedisDemoDb;
GO

CREATE TABLE Products
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100),
    Price DECIMAL(10,2)
)

GO

INSERT INTO Products (Name, Price) VALUES ('Laptop', 75000)
INSERT INTO Products (Name, Price) VALUES ('Mouse', 500)
INSERT INTO Products (Name, Price) VALUES ('Keyboard', 1500)
INSERT INTO Products (Name, Price) VALUES ('Monitor', 12000)
GO

SELECT * FROM Products;
GO