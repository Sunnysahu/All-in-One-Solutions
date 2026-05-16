## Setup Instructions

### 1. Install Required Packages

Run the following commands in your project directory:

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

---

### 2. Create the Database

Run the following SQL script in SQL Server Management Studio (SSMS):

```sql
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
```

---

### 3. Run the Application

Start the API project:

```bash
dotnet run
```

---

### 4. Test the API in Postman

Use Postman to hit the endpoint:

```http
POST /api/logs
```

Example request body:

```json
{
  "message": "Processing started",
  "status": "Success",
  "stepNumber": 1
}
```

---

### 5. Verify Database Entries

Run the following query to check inserted logs:

```sql
SELECT *
FROM ProcessingLogs
ORDER BY Id DESC;
```
