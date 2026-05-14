# API Versioning Guide (.NET Web API)

## Change API Version from Swagger UI

In **Swagger**, use the **top-left version selector** to switch between API versions.

## Test Using Postman

### Version 1 (v1)
````md 

```http
https://localhost:7113/api/Products/GetAll
````

### Version 2 (v2)

```http
https://localhost:7113/api/v2/Products/GetAll
```

---

# Using Multiple API Versions in the Same Controller

If you want to support **multiple API versions inside the same controller**, configure versioning directly in the controller file.

## Example

```csharp
[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    // Version 1 Endpoint
    [HttpGet]
    [Route("GetAll")]
    [MapToApiVersion("1.0")]
    public IActionResult GetAllV1()
    {
        return Ok("Products from API Version 1");
    }

    // Version 2 Endpoint
    [HttpGet]
    [Route("GetAll")]
    [MapToApiVersion("2.0")]
    public IActionResult GetAllV2()
    {
        return Ok("Products from API Version 2");
    }
}
```

---

# Key Attributes Explained

| Attribute                                           | Purpose                                 |
| --------------------------------------------------- | --------------------------------------- |
| `[ApiVersion("1.0")]`                               | Defines supported API versions          |
| `[Route("api/v{version:apiVersion}/[controller]")]` | Adds version number in the route        |
| `[MapToApiVersion("1.0")]`                          | Maps a specific action to API version 1 |
| `[MapToApiVersion("2.0")]`                          | Maps a specific action to API version 2 |

---

# Resulting Endpoints

| Version | Endpoint                  |
| ------- | ------------------------- |
| v1      | `/api/v1/Products/GetAll` |
| v2      | `/api/v2/Products/GetAll` |

---

```
```
