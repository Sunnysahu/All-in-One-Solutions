# Razorpay Webhook Testing Guide

This guide helps you test the Razorpay webhook API locally using HMAC SHA256 signature validation.

---

# Webhook Endpoint

```http
POST https://localhost:7034/api/Webhook/razorpay
```

---

# Generate HMAC Signature

Use:

https://www.freeformatter.com/hmac-generator.html#before-output

## Configuration

| Field | Value |
|---|---|
| Secret Key | `Sunny` |
| Digest Algorithm | `SHA256` |

---

# Request Payload

Use this exact JSON payload:

```json
{
    "event": "payment.captured",
    "account_id": "acc_001",
    "payload": {
        "payment": {
            "entity": {
                "id": "pay_001",
                "order_id": "order_001",
                "amount": 5000,
                "currency": "INR",
                "status": "captured"
            }
        }
    }
}
```

---

# Required Headers

| Header | Value |
|---|---|
| `X-Webhook-Id` | `evt_1001` |
| `X-Razorpay-Signature` | `<Generated HMAC Hash>` |

Example:

```http
X-Razorpay-Signature: ab740b0f98d602192bae42f96aa6fe6b502a08c8068d0f962bb09ffe584c527b
```

---

# Test Cases

---

# TEST CASE 1 — SUCCESSFUL WEBHOOK

## Purpose

Verify:

- Signature validation
- Database insert
- Queue processing
- Background worker execution

---

## Request

### Endpoint

```http
POST https://localhost:7034/api/Webhook/razorpay
```

### Headers

```http
X-Webhook-Id: evt_1001
X-Razorpay-Signature: <Generated HMAC Hash>
```

### Body

```json
{
    "event": "payment.captured",
    "account_id": "acc_001",
    "payload": {
        "payment": {
            "entity": {
                "id": "pay_001",
                "order_id": "order_001",
                "amount": 5000,
                "currency": "INR",
                "status": "captured"
            }
        }
    }
}
```

---

## Expected Response

```http
200 OK
```

---

# TEST CASE 2 — INVALID SIGNATURE

## Purpose

Verify webhook security.

---

## Steps

Change the `X-Razorpay-Signature` header value to any invalid string.

Example:

```http
X-Razorpay-Signature: invalid_signature
```

---

## Expected Response

```http
401 Unauthorized
```

---

# TEST CASE 3 — DUPLICATE WEBHOOK

## Purpose

Verify idempotency handling.

Real payment providers retry webhook delivery multiple times.

Without duplicate protection:

- Customer may be charged twice
- Wallet balance may be credited twice
- Orders may be shipped multiple times

This is a critical production safety check.

---

## Steps

Send the **same request again** with:

```http
X-Webhook-Id: evt_1001
```

Use the exact same:

- Payload
- Signature
- Event ID

---

## Expected Response

```http
200 OK
```

---

## Internal Expected Behavior

The webhook should be ignored internally as a duplicate.

Example log:

```text
Duplicate webhook ignored
```

---

## Database Verification

Run:

```sql
SELECT COUNT(*) FROM Payments;
```

### Expected Result

```text
1
```

### NOT

```text
2
```

---

# TEST CASE 4 — MISSING SIGNATURE HEADER

## Purpose

Verify mandatory signature validation.

---

## Steps

Remove this header entirely:

```http
X-Razorpay-Signature
```

---

## Expected Response

```http
401 Unauthorized
```

---

# SQL Server Database Setup

Run the following SQL script to reset and recreate the database.

```sql
USE master;
GO

ALTER DATABASE WebhookDemo
SET SINGLE_USER
WITH ROLLBACK IMMEDIATE;
GO

DROP DATABASE WebhookDemo;
GO

CREATE DATABASE WebhookDemo;
GO

USE WebhookDemo;
GO

CREATE TABLE Payments
(
    Id INT PRIMARY KEY IDENTITY,
    OrderId NVARCHAR(100) NOT NULL,
    PaymentId NVARCHAR(100) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Currency NVARCHAR(10) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE TABLE WebhookEvents
(
    Id BIGINT PRIMARY KEY IDENTITY,
    EventId NVARCHAR(200) NOT NULL,
    EventType NVARCHAR(100) NOT NULL,
    Payload NVARCHAR(MAX) NOT NULL,
    Signature NVARCHAR(500) NOT NULL,
    IsProcessed BIT NOT NULL DEFAULT 0,
    RetryCount INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ProcessedAt DATETIME2 NULL
);
GO

CREATE TABLE ProcessedWebhooks
(
    Id BIGINT PRIMARY KEY IDENTITY,
    EventId NVARCHAR(200) NOT NULL UNIQUE,
    ProcessedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

CREATE UNIQUE INDEX IX_ProcessedWebhooks_EventId
ON ProcessedWebhooks(EventId);
GO

SELECT * FROM Payments;
SELECT * FROM WebhookEvents;
SELECT * FROM ProcessedWebhooks;
```

---

# Expected Database Flow

| Table | Purpose |
|---|---|
| `Payments` | Stores successful payment records |
| `WebhookEvents` | Stores incoming webhook payloads |
| `ProcessedWebhooks` | Prevents duplicate webhook processing |

---

# Quick Testing Checklist

| Test Case | Expected Result |
|---|---|
| Valid webhook | `200 OK` |
| Invalid signature | `401 Unauthorized` |
| Missing signature | `401 Unauthorized` |
| Duplicate webhook | Ignored internally |
| Payments count after duplicate | `1` row only |

---

# Recommended Testing Tools

- Postman
- Bruno
- cURL
- Swagger

---

# Example cURL Request

```bash
curl --location 'https://localhost:7034/api/Webhook/razorpay' \
--header 'Content-Type: application/json' \
--header 'X-Webhook-Id: evt_1001' \
--header 'X-Razorpay-Signature: YOUR_HMAC_SIGNATURE' \
--data '{
    "event": "payment.captured",
    "account_id": "acc_001",
    "payload": {
        "payment": {
            "entity": {
                "id": "pay_001",
                "order_id": "order_001",
                "amount": 5000,
                "currency": "INR",
                "status": "captured"
            }
        }
    }
}'
```

---

# Postman Collection Runner Testing

To run automated tests in Postman:

1. Add the API request to a Collection
2. Right-click the Collection
3. Click **Run Collection**
4. Set the number of iterations
5. Start the run

---

## Important

Enable the **HMAC** option and keep it always enabled during testing.

---

# Postman Pre-request Script

Paste this into the **Scripts** tab.

```javascript
const randomNumber = Math.floor(Math.random() * 100000);

pm.variables.set("eventId", `evt_${randomNumber}`);
pm.variables.set("paymentId", `pay_${randomNumber}`);
pm.variables.set("orderId", `order_${randomNumber}`);
```

---

# Postman Raw JSON Body

Paste this into **Body → raw → JSON**

```json
{
    "event": "payment.captured",
    "account_id": "acc_001",
    "payload": {
        "payment": {
            "entity": {
                "id": "{{paymentId}}",
                "order_id": "{{orderId}}",
                "amount": 5000,
                "currency": "INR",
                "status": "captured"
            }
        }
    }
}
```

------------------------------

# How to Make Multiple Workers in .NET BackgroundService

## Simplest Production-Style Approach

Create multiple tasks inside `ExecuteAsync()`.

```csharp
protected override async Task ExecuteAsync(
    CancellationToken stoppingToken)
{
    var workers = new List<Task>();

    for (int i = 1; i <= 5; i++)
    {
        int workerId = i;

        workers.Add(Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation(
                        "Worker {WorkerId} waiting...",
                        workerId);

                    var webhookEvent =
                        await _queue.DequeueAsync(stoppingToken);

                    _logger.LogInformation(
                        "Worker {WorkerId} picked EventId: {EventId}",
                        workerId,
                        webhookEvent.EventId);

                    await Task.Delay(10000);

                    using var scope =
                        _serviceScopeFactory.CreateScope();

                    var webhookService =
                        scope.ServiceProvider
                            .GetRequiredService<IWebhookService>();

                    await webhookService
                        .ProcessWebhookAsync(webhookEvent);

                    _logger.LogInformation(
                        "Worker {WorkerId} completed EventId: {EventId}",
                        workerId,
                        webhookEvent.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Worker failed");
                }
            }
        }, stoppingToken));
    }

    await Task.WhenAll(workers);
}
```

---

# What This Does

Creates:

- Worker 1
- Worker 2
- Worker 3
- Worker 4
- Worker 5

All listening to the **same queue**.

---

# How `Channel<T>` Behaves

Suppose the queue contains:

```text
Event1
Event2
Event3
Event4
Event5
```

Workers automatically compete.

Example:

```text
Worker1 → Event1
Worker2 → Event2
Worker3 → Event3
Worker4 → Event4
Worker5 → Event5
```

## Important

Each message is picked **ONLY ONCE**.

`Channel<T>` safely distributes messages across workers.

This is called:

## Producer-Consumer Pattern

---

# What You Will Observe

## Before

```text
10 requests
10 sec delay
100 sec total
```

## Now with 5 Workers

```text
roughly 20 sec total
```

Because:

- 5 events processed simultaneously
- parallel processing occurs

---

# Very Important Production Concept

Each worker creates its own scope:

```csharp
using var scope =
    _serviceScopeFactory.CreateScope();
```

This is extremely important because:

- each worker gets a separate `DbContext`
- `DbContext` is NOT thread-safe

## NEVER SHARE SAME DbContext Across Workers

### Bad

Multiple threads using the same `DbContext`.

Can cause:

- crashes
- race conditions
- corrupted tracking

---

# Real Enterprise Concept

This is called:

## Concurrent Consumers

Used in:

- RabbitMQ consumers
- Kafka consumers
- Azure Service Bus processors
- Hangfire workers

---

# How Many Workers Should You Use?

Depends on:

- CPU
- DB capacity
- external API rate limits
- memory
- workload type

## Common Production Numbers

Webhook systems often use:

- 5
- 10
- 20
- 50

workers.

---

# Important Difference

## More Workers != More Speed Always

Too many workers can:

- overload DB
- exhaust connections
- increase contention
- create deadlocks

Real systems tune worker count carefully.

---

# What You Should Test Now

Send:

```text
10 requests
with 10 sec delay
```

Observe logs:

```text
Worker 1 picked evt_1001
Worker 2 picked evt_1002
Worker 3 picked evt_1003
```

simultaneously.

This proves:

- multiple consumers
- parallel processing
- concurrent queue handling

are working correctly.

---

# Important DbContext Clarification

`DbContext` is still shared **within each worker**, but NOT across workers.

So add this line:

```csharp
var webhookService =
    scope.ServiceProvider
        .GetRequiredService<IWebhookService>();
```

together with:

```csharp
using var scope =
    _serviceScopeFactory.CreateScope();
```

This creates:

- separate `DbContext`
- separate scoped services
- separate EF tracking

per worker.

That is the correct enterprise-safe pattern.

---

# Why?

Because:

## DbContext is NOT thread-safe

Internally EF Core has:

- change tracker
- entity states
- DB connection usage

Multiple threads modifying the same `DbContext` can corrupt state.

Example:

```csharp
private readonly ApplicationDbContext _dbContext;
```

used by:

- Worker1
- Worker2
- Worker3

simultaneously.

Can cause:

- random exceptions
- wrong updates
- tracking corruption
- duplicate inserts

---

# Hangfire Notes

Install these packages for GUI-based background job status and logs:

```text
Hangfire
Hangfire.SqlServer
```

## Important

Hangfire handles dequeue internally.

It also stores jobs in SQL Server.

---

# What Happens Exactly

Suppose:

```text
100 jobs queued
20 processed
80 remaining
```

Then you stop the server.

## Result

- Workers stop
- Remaining jobs stay in SQL
- Jobs resume automatically when server starts again

Open this URL : [Hangfire Dashboard](https://localhost:7034/hangfire/) or `https://localhost:7034/hangfire/`