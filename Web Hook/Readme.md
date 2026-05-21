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

[FreeFormatter HMAC Generator](https://www.freeformatter.com/hmac-generator.html?utm_source=chatgpt.com#before-output)

## Configuration

| Field            | Value    |
| ---------------- | -------- |
| Secret Key       | `Sunny`  |
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

| Header                 | Value                   |
| ---------------------- | ----------------------- |
| `X-Webhook-Id`         | `evt_1001`              |
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

* Signature validation
* Database insert
* Queue processing
* Background worker execution

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

* Customer may be charged twice
* Wallet balance may be credited twice
* Orders may be shipped multiple times

This is a critical production safety check.

---

## Steps

Send the **same request again** with:

```http
X-Webhook-Id: evt_1001
```

Use the exact same:

* Payload
* Signature
* Event ID

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

| Table               | Purpose                               |
| ------------------- | ------------------------------------- |
| `Payments`          | Stores successful payment records     |
| `WebhookEvents`     | Stores incoming webhook payloads      |
| `ProcessedWebhooks` | Prevents duplicate webhook processing |

---

# Quick Testing Checklist

| Test Case                      | Expected Result    |
| ------------------------------ | ------------------ |
| Valid webhook                  | `200 OK`           |
| Invalid signature              | `401 Unauthorized` |
| Missing signature              | `401 Unauthorized` |
| Duplicate webhook              | Ignored internally |
| Payments count after duplicate | `1` row only       |

---

# Recommended Testing Tools

* Postman
* Bruno
* cURL
* Swagger

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


### To Run Test in Postman, Add API Endpoint to Collections then Right Click then Run, add No. of Iterations then See