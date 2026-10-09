CREATE DATABASE BloomFilterDemo;
GO

USE BloomFilterDemo;
GO

CREATE TABLE Products
(
    Id INT PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);
GO

INSERT INTO Products (Id, Name)
VALUES
(1, 'Product 1'),
(2, 'Product 2'),
(3, 'Product 3'),
(4, 'Product 4'),
(5, 'Product 5');
GO