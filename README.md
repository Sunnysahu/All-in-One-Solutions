# Clean Architecture Exception Handling Notes (.NET Core)

```text
Repository  -> DB operations
Service     -> Business logic + throw exceptions
Controller  -> HTTP success responses
Middleware  -> Global error handling
```

---

# 1. Custom Response Model

```csharp
public class ApiResponse<T>
{
    public int StatusCode { get; set; }

    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }
}
```

---

# 2. Custom Exception

## Application/Exceptions/NotFoundException.cs

```csharp
public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }
}
```

---

# 3. Service Layer

```csharp
public async Task<UserDto> GetUserAsync(int id)
{
    var user = await _repo.GetByIdAsync(id);

    if (user == null)
    {
        throw new NotFoundException("User not found");
    }

    return new UserDto
    {
        Id = user.Id,
        Name = user.Name
    };
}
```

---

# 4. Exception Middleware

## API/Middleware/ExceptionMiddleware.cs

```csharp
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = 404;

            var response = new ApiResponse<object>
            {
                StatusCode = 404,
                Success = false,
                Message = ex.Message,
                Data = null
            };

            await context.Response.WriteAsJsonAsync(response);
        }
        catch (Exception)
        {
            context.Response.StatusCode = 500;

            var response = new ApiResponse<object>
            {
                StatusCode = 500,
                Success = false,
                Message = "Internal Server Error",
                Data = null
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
```

---

# 5. Register Middleware

## Program.cs

```csharp
app.UseMiddleware<ExceptionMiddleware>();
```

Place before:

```csharp
app.MapControllers();
```

---

# 6. Controller

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> Get(int id)
{
    var user = await _service.GetUserAsync(id);

    var response = new ApiResponse<UserDto>
    {
        StatusCode = 200,
        Success = true,
        Message = "User fetched successfully",
        Data = user
    };

    return Ok(response);
}
```

---

# Success Flow

```text
Controller
   ↓
Service returns DTO
   ↓
Controller returns Ok()
```

Response:

```json
{
  "statusCode": 200,
  "success": true,
  "message": "User fetched successfully",
  "data": {}
}
```

---

# Error Flow

```text
Service throws exception
   ↓
Controller stops immediately
   ↓
Middleware catches exception
   ↓
Middleware sends error response
```

Response:

```json
{
  "statusCode": 404,
  "success": false,
  "message": "User not found",
  "data": null
}
```

---

# Important Rules

```text
Repository -> return entity/null

Service -> throw exceptions

Controller -> success responses

Middleware -> error responses
```

---

# Important Understanding

```text
throw = stop execution immediately

After throw:
    next line NEVER runs

Exception jumps upward to nearest catch block
```
