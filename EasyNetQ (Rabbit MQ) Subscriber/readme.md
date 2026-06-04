# EasyNetQ Message Consumption Patterns

These are two common message consumption patterns in EasyNetQ.

---

# 1. Manual Subscription

Your `ManualSubscriberWorker` uses:

```csharp
await bus.PubSub.SubscribeAsync<OrderPlacedMessage>(
    subscriptionId: "order_demo",
    onMessage: HandleOrderAsync,
    cancellationToken: stoppingToken
);
```

This is called:

> **Manual Subscription (Programmatic Subscription)**

because you explicitly define:

```text
Subscribe to OrderPlacedMessage
→ Call HandleOrderAsync when a message arrives
```

---

## Flow

```text
Startup
   ↓
SubscribeAsync()
   ↓
Message arrives
   ↓
HandleOrderAsync(message)
```

---

## Pros

* Simple
* Easy to understand
* Good for small applications
* Everything is explicit and visible

---

## Cons

* Lots of boilerplate for many message types
* Hard to scale as the system grows

Example scaling issue:

```csharp
SubscribeAsync<OrderPlacedMessage>(...)
SubscribeAsync<OrderCancelledMessage>(...)
SubscribeAsync<CustomerCreatedMessage>(...)
SubscribeAsync<PaymentProcessedMessage>(...)
```

---

# 2. AutoSubscriber Pattern

Your second approach uses:

```csharp
var autoSubscriber = new AutoSubscriber(
    bus,
    serviceProvider,
    "order-demo"
);

await autoSubscriber.SubscribeAsync(
    typeof(OrderSubscriberWorker).Assembly.GetTypes(),
    stoppingToken
);
```

And consumers like:

```csharp
public class OrderPlacedConsumer
    : IConsumeAsync<OrderPlacedMessage>
{
    public Task ConsumeAsync(OrderPlacedMessage message)
    {
        ...
    }
}
```

This is called:

> **Convention-Based Subscription (Auto Discovery Pattern)**

because EasyNetQ automatically discovers consumers.

---

## Flow

```text
Startup
   ↓
Scan Assembly
   ↓
Find IConsumeAsync<T>
   ↓
Create subscriptions automatically
   ↓
Message arrives
   ↓
ConsumeAsync(message)
```

---

## Pros

* No manual subscription wiring
* Scales very well
* Clean separation of consumers
* Great for microservices

---

## Cons

* More “magic” / less explicit
* Harder to debug initially
* Requires understanding conventions

---

# Side-by-Side Comparison

| Manual Subscription   | AutoSubscriber       |
| --------------------- | -------------------- |
| Explicit setup        | Convention-based     |
| `SubscribeAsync<T>()` | Auto discovery       |
| Handler methods       | Consumer classes     |
| Good for small apps   | Good for large apps  |
| Less abstraction      | More abstraction     |
| Easier debugging      | More hidden behavior |

---

# Code Comparison

## Manual Subscription

```csharp
public class ManualSubscriberWorker
{
    protected override async Task ExecuteAsync(...)
    {
        await bus.PubSub.SubscribeAsync<OrderPlacedMessage>(
            "order_demo",
            HandleOrderAsync
        );
    }

    private Task HandleOrderAsync(OrderPlacedMessage message)
    {
        // handle message
    }
}
```

You explicitly register each handler.

---

## AutoSubscriber

```csharp
public class OrderPlacedConsumer
    : IConsumeAsync<OrderPlacedMessage>
{
    public Task ConsumeAsync(OrderPlacedMessage message)
    {
        // handle message
    }
}
```

And registration happens automatically:

```csharp
await autoSubscriber.SubscribeAsync(
    typeof(OrderPlacedConsumer).Assembly.GetTypes(),
    stoppingToken
);
```

---

# Similarity with ASP.NET Core

## Manual Subscription

Like explicitly mapping endpoints:

```csharp
app.MapGet("/orders", GetOrders);
app.MapPost("/orders", CreateOrder);
```

---

## Auto Discovery

Like controllers:

```csharp
public class OrdersController : Controller
{
}
```

ASP.NET automatically discovers controllers.

---

# When to Use What

## Small Projects

Use:

```text
Manual Subscription
```

because it is simple and explicit.

---

## Medium / Large Systems (Microservices)

Use:

```text
AutoSubscriber
```

because you may have many consumers like:

* OrderPlacedConsumer
* PaymentProcessedConsumer
* CustomerCreatedConsumer
* InventoryReservedConsumer

and manual registration becomes unmanageable.

---

# Summary

There are two patterns:

### 1. Manual Subscription

You explicitly register every handler using `SubscribeAsync<T>()`.

### 2. AutoSubscriber Pattern

EasyNetQ scans assemblies and automatically registers consumers using `IConsumeAsync<T>`.