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
