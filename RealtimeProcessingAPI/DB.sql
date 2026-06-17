CREATE DATABASE CleanStreamDB;
GO

USE CleanStreamDB;
GO

CREATE TABLE ProcessingLogs
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    Message NVARCHAR(500) NOT NULL,

    Status NVARCHAR(100) NULL,

    StepNumber INT NOT NULL,

    CreatedAt DATETIME2 NOT NULL
        DEFAULT SYSUTCDATETIME()
);
GO


CREATE NONCLUSTERED INDEX IX_ProcessingLogs_CreatedAt
ON ProcessingLogs(CreatedAt DESC);
GO

SELECT *
FROM ProcessingLogs
ORDER BY Id;
GO