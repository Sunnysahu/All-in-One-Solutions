CREATE DATABASE OutboxPatternDb;
GO

USE OutboxPatternDb;
GO

--------------------------------------------------
-- Orders
--------------------------------------------------

CREATE TABLE Orders
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    ProductName NVARCHAR(200) NOT NULL,

    Price DECIMAL(18,2) NOT NULL,

    CreatedAt DATETIME2 NOT NULL
);
GO

--------------------------------------------------
-- OutboxMessages
--------------------------------------------------

CREATE TABLE OutboxMessages
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,

    EventType NVARCHAR(100) NOT NULL,

    Payload NVARCHAR(MAX) NOT NULL,

    IsProcessed BIT NOT NULL DEFAULT 0,

    CreatedAt DATETIME2 NOT NULL
);
GO

--------------------------------------------------
-- ProcessedMessages
--------------------------------------------------

CREATE TABLE ProcessedMessages
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,

    ProcessedAt DATETIME2 NOT NULL
);
GO

SELECT * FROM Orders;

SELECT * FROM OutboxMessages;

SELECT * FROM ProcessedMessages;