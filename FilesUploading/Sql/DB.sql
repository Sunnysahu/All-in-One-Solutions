CREATE DATABASE ResumableUploadDB
GO
USE ResumableUploadDB
-- DROP DATABASE ResumableUploadDB

Use Master

CREATE TABLE Files (
    Id INT IDENTITY PRIMARY KEY,
    FileName NVARCHAR(255) NOT NULL,
    OriginalFileName NVARCHAR(255) NOT NULL,
    FileSize BIGINT NOT NULL,
    TotalChunks INT NOT NULL,
    UploadedChunks INT NOT NULL DEFAULT 0,
    Status NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

 --DROP TABLE Files;

CREATE TABLE FileChunks (
    Id INT IDENTITY PRIMARY KEY,
    FileId INT NOT NULL,
    ChunkIndex INT NOT NULL,
    ChunkSize BIGINT NOT NULL,
    UploadedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_FileChunks_File
    FOREIGN KEY (FileId) REFERENCES Files(Id),

    CONSTRAINT UQ_File_Chunk UNIQUE(FileId, ChunkIndex)
);
GO

SELECT * FROM Files; SELECT * FROM FileChunks

DROP TABLE Files; DROP TABLE FileChunks;