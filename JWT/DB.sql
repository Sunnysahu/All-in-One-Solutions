CREATE DATABASE JwtAuthDb;

GO

USE JwtAuthDb;

GO

CREATE TABLE Users (
    Id INT IDENTITY PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Password NVARCHAR(200) NOT NULL,
    Role NVARCHAR(50) NOT NULL
);

Go

INSERT INTO Users (Username, Password, Role)
VALUES 
('admin','admin123','Admin'),
('sunny','123456','User'),
('manager','manager123','Manager')

GO 
CREATE TABLE RefreshTokens (
    Id INT IDENTITY PRIMARY KEY,
    UserId INT NOT NULL,
    Token NVARCHAR(500) NOT NULL,
    ExpiryDate DATETIME NOT NULL,
    IsRevoked BIT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (UserId) REFERENCES Users(Id)
);


SELECT * FROM Users; SELECT * FROM RefreshTokens;


delete  from RefreshTokens;
GO