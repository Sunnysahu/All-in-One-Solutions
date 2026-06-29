/*

USE master;
GO

ALTER DATABASE OutboxProductionDb
SET SINGLE_USER
WITH ROLLBACK IMMEDIATE;
GO

DROP database OutboxProductionDb
GO

CREATE DATABASE OutboxProductionDb;
GO

USE OutboxProductionDb;
GO


CREATE TABLE Orders
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ProductName NVARCHAR(200),
    Quantity INT,
    CreatedAt DATETIME2
);
GO

CREATE TABLE OutboxMessages
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,

    EventType NVARCHAR(200),

    Payload NVARCHAR(MAX),

    Status INT,

    RetryCount INT,

    LockedBy NVARCHAR(100),

    LockedAt DATETIME2,

    CreatedAt DATETIME2,

    ProcessedAt DATETIME2 NULL
);
GO

CREATE TABLE ProcessedMessages
(
    MessageId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    ProcessedAt DATETIME2 NOT NULL
);
GO

CREATE TYPE GuidListType AS TABLE
(
    Id UNIQUEIDENTIFIER PRIMARY KEY
);
GO

    SELECT * FROM Orders;
    SELECT * FROM OutboxMessages

    SELECT * FROM ProcessedMessages
    SELECT COUNT(*) as Total_Processed_Message FROM ProcessedMessages ;



SELECT
    Id,
    Status,
    LockedBy,
    LockedAt
FROM OutboxMessages where status = 0
ORDER BY CreatedAt;


*/