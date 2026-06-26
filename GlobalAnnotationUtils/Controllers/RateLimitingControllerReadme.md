# ASP.NET Core Rate Limiting Examples

## 1. Fixed Window Limiter

### Program.cs

// Error After 5 Continue Request

```csharp
using System.Threading.RateLimiting;

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(
        "fixed",
        config =>
        {
            config.PermitLimit = 5;
            config.Window = TimeSpan.FromMinutes(1);
            config.QueueLimit = 0;
        });
});

app.UseRateLimiter();
```

### Controller

```csharp
[EnableRateLimiting("fixed")]
[HttpGet]
public IActionResult Get()
{
    return Ok("Success");
}
```

### Test

* Requests 1–5 → `200 OK`
* Request 6 → `429 Too Many Requests`

---

## 2. Sliding Window Limiter

### Program.cs

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddSlidingWindowLimiter(
        "sliding",
        config =>
        {
            config.PermitLimit = 5;
            config.Window = TimeSpan.FromMinutes(1);
            config.SegmentsPerWindow = 6;
            config.QueueLimit = 0;
        });
});
```

### Controller

```csharp
[EnableRateLimiting("sliding")]
```

### Test

1. Send 5 requests quickly.
2. Wait 10 seconds.
3. Some permits become available gradually.

**Behavior:** Unlike a fixed window, permits are restored gradually instead of resetting all at once.

---

## 3. Token Bucket Limiter

### Program.cs

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddTokenBucketLimiter(
        "token",
        config =>
        {
            config.TokenLimit = 10;
            config.TokensPerPeriod = 2;
            config.ReplenishmentPeriod = TimeSpan.FromSeconds(5);
            config.QueueLimit = 0;
            config.AutoReplenishment = true;
        });
});
```

### Controller

```csharp
[EnableRateLimiting("token")]
```

### Test

* First 10 requests → `200 OK`
* Request 11 → `429 Too Many Requests`

Wait 5 seconds:

* 2 new requests become available.

---

# Part 4: User-Based Rate Limiting

A common production scenario where each authenticated user gets an independent rate limit.

### Program.cs

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("user-policy", context =>
    {
        var userId =
            context.User.Identity?.Name ??
            "anonymous";

        return RateLimitPartition.GetFixedWindowLimiter(
            userId,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1)
            });
    });
});
```

### Controller

```csharp
[EnableRateLimiting("user-policy")]
```

### Behavior

Each user gets their own request counter.

Example:

* User A → 5 requests allowed
* User B → 5 requests allowed

Their limits do not affect each other.

---

# Part 5: IP-Based Rate Limiting

Limits requests based on the client's IP address.

### Program.cs

```csharp
options.AddPolicy("ip-policy", context =>
{
    var ip =
        context.Connection.RemoteIpAddress?.ToString()
        ?? "unknown";

    return RateLimitPartition.GetFixedWindowLimiter(
        ip,
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1)
        });
});
```

### Controller

```csharp
[EnableRateLimiting("ip-policy")]
```

### Behavior

Each IP address receives its own rate limit bucket.

---

# Part 6: Concurrent Request Limiter

Restricts how many requests can execute simultaneously.

### Program.cs

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddConcurrencyLimiter(
        "concurrent",
        config =>
        {
            config.PermitLimit = 2;
            config.QueueLimit = 0;
        });
});
```

### Controller

```csharp
[EnableRateLimiting("concurrent")]
```

### Test

* 2 requests can run concurrently.
* A 3rd concurrent request receives `429 Too Many Requests`.
* Once one request completes, another request can enter.

### Use Cases

* Protecting expensive database operations
* Limiting CPU-intensive endpoints
* Preventing resource exhaustion

---

## Summary

| Limiter Type   | Best For                 | Key Behavior                      |
| -------------- | ------------------------ | --------------------------------- |
| Fixed Window   | Simple APIs              | Counter resets at fixed intervals |
| Sliding Window | Smoother traffic control | Gradual permit recovery           |
| Token Bucket   | Burst traffic            | Allows bursts, refills over time  |
| User-Based     | Authenticated APIs       | Separate limits per user          |
| IP-Based       | Public APIs              | Separate limits per IP address    |
| Concurrency    | Expensive operations     | Limits simultaneous requests      |

```
```
