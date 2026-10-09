using BloomFilter;
using BloomFilterDemo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloomFilterDemo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BloomFilterPackageController(AppDbContext db) : ControllerBase
{
    private readonly AppDbContext _db = db;

    private static readonly IBloomFilter _filter = FilterBuilder.Build(1000, 0.01);

    [HttpPost("load")]
    public async Task<IActionResult> Load()
    {
        var productIds = await _db.Products.AsNoTracking().Select(x => x.Id).ToListAsync();

        foreach (var id in productIds)
        {
            _filter.Add(id.ToString());
        }

        return Ok(new
        {
            message = "Package Bloom Filter loaded.",
            count = productIds.Count
        });
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        if (!_filter.Contains(id.ToString()))
        {
            return NotFound(new
            {
                message = "Product definitely does not exist."
            });
        }

        var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (product == null)
        {
            return NotFound(new
            {
                message = "Bloom Filter says possibly exists, but product was not found in the database."
            });
        }

        return Ok(product);
    }

}

