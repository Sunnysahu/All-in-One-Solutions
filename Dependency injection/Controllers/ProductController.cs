using Dependency_injection.Models;
using Dependency_injection.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var products = _service.GetProducts();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var product = _service.GetProduct(id);
        return Ok(product);
    }

    [HttpPost]
    public IActionResult Create(Product product)
    {
       _service.CreateProduct(product);
        return Ok(new
        {
            Message = "Product Created Successfully",
            StatusCode = StatusCode(200, new { msg = "Created"}),
            
        });
    }
}