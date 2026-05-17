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


NOTE : 

```
public async IAsyncEnumerable<ProgressDto> ProcessAsync ([System.Runtime.CompilerServices.EnumeratorCancellation]CancellationToken cancellationToken)
```

Problem

CancellationToken inside async streams behaves differently.
Got you — here’s a **single clean, copy-pasteable README.md file**:


`[EnumeratorCancellation]`


A very advanced but important concept in async streams.

### The Problem

`CancellationToken` behaves differently inside async streams (`IAsyncEnumerable`).

With normal async methods:

```csharp
Task MyMethod(CancellationToken token)
````

The token simply cancels the method execution.

But async streams are different:

An `IAsyncEnumerable`:

* pauses execution multiple times
* resumes multiple times
* yields values over time
* contains multiple async await points

So the compiler needs help understanding:

> Which token controls the async enumeration?

---

### What `[EnumeratorCancellation]` Does

```csharp
async IAsyncEnumerable<string> StreamData(
    [EnumeratorCancellation] CancellationToken token = default)
```

It tells the compiler:

> This token controls async stream cancellation.

---

### Without It

* cancellation may not propagate correctly
* stream may continue running
* background work may continue unnecessarily

---

## `FlushAsync()`

### Problem

ASP.NET Core buffers responses by default.

That means data is:

* stored in memory first
* sent later in chunks

---

### Solution

```csharp
await Response.Body.FlushAsync();
```

This forces immediate sending of buffered data.

---

### Why it matters

Used in:

* SSE (Server-Sent Events)
* AI streaming responses
* live logs
* real-time updates

Without it, users may see delayed output.

---

## `text/event-stream`

```csharp
Response.ContentType = "text/event-stream";
```

This enables Server-Sent Events (SSE).

It keeps the connection open and streams data like:

```
data: Hello

data: Streaming...

data: Done
```

---

## Summary

| Concept                    | Purpose                           |
| -------------------------- | --------------------------------- |
| `[EnumeratorCancellation]` | Proper async stream cancellation  |
| `FlushAsync()`             | Forces immediate response sending |
| `text/event-stream`        | Enables SSE streaming             |
