# RabbitMQ + MassTransit in .NET

## SQL SERVER CODE
```
CREATE DATABASE OrderDb;
GO

USE OrderDb;
GO

CREATE TABLE Orders
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(200),
    Quantity INT,
    Price DECIMAL(18,2),
    [Status] NVARCHAR(50),
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO

select * from Orders;
```

> Just Hit the Endpoint and Check Console

## What Problem Does This Solve?

### Without Queues

```text
Client → API → Heavy Processing → Database → Email → Third Party API
```

### Problems

* Slow API response
* Request timeout
* System crashes under load
* Retry handling difficult
* Tight coupling

---

## With RabbitMQ + MassTransit

```text
Client → API → Queue → Consumer/Worker → Processing
```

### Benefits

* API responds instantly
* Background processing happens separately
* Automatic retries
* Scalable
* Fault tolerant
* Production-ready

---

# Real Production Use Cases

## Example 1: Payment System

### Flow

1. User places order

2. API:

   * Saves order
   * Publishes `OrderCreated` event
   * Returns `200 OK` immediately

3. Background Consumer:

   * Sends email
   * Updates inventory
   * Calls payment gateway
   * Generates invoice

---

## Example 2: Webhook Processing

### Webhook arrives from:

* Stripe
* Razorpay
* GitHub
* Shopify

### API:

* Stores webhook
* Publishes message
* Returns `200 OK` fast

### Consumer:

* Validates
* Processes
* Retries failures
* Logs everything

---

# What is MassTransit?

[MassTransit Official Website](https://masstransit.io?utm_source=chatgpt.com)

MassTransit is a .NET abstraction layer over message brokers.

Instead of writing raw RabbitMQ code for:

* Queue declaration
* Exchange binding
* Serialization
* Retries
* Dead letter queues

MassTransit handles them automatically.

---

# What is RabbitMQ?

[RabbitMQ Official Website](https://www.rabbitmq.com?utm_source=chatgpt.com)

RabbitMQ is a message broker.

It stores messages temporarily between:

* Producer
* Consumer

It guarantees message delivery.

---

# Core Concepts

## Producer

Publishes messages.

### Example

```csharp
await _publishEndpoint.Publish(new OrderCreatedEvent
{
    OrderId = 1
});
```

---

## Queue

Stores messages safely until consumed.

---

## Consumer

Reads messages from queue.

### Example

```csharp
public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;

        Console.WriteLine($"Processing Order: {message.OrderId}");

        await Task.CompletedTask;
    }
}
```

---

## Exchange

RabbitMQ routing mechanism.

MassTransit auto-creates it.

---

## Retry

If processing fails:

* Retry automatically
* No message loss

---

## Dead Letter Queue (DLQ)

If retries are exhausted:

* Move failed message
* Investigate later

---

# RabbitMQ Docker Setup

## docker-compose.yml

```yaml
version: '3.8'

services:
  rabbitmq:
    image: rabbitmq:3-management
    container_name: rabbitmq
    ports:
      - "5672:5672"
      - "15672:15672"
```

---

# RabbitMQ Management UI

Open:

```text
http://localhost:15672
```

### Default Credentials

```text
username: guest
password: guest
```

---

# Summary

Using RabbitMQ + MassTransit helps build:

* Asynchronous systems
* Reliable background processing
* Scalable microservices
* Fault-tolerant applications
* Event-driven architectures

It is widely used in:

* E-commerce
* FinTech
* Notification systems
* Webhook processing
* Distributed systems
* Enterprise applications
