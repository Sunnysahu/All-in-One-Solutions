using BloomFilterDemo.BloomFilter;
using BloomFilterDemo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloomFilterDemo.Controllers;

public class BloomFilterFromScratchController(AppDbContext db) : ControllerBase
{
    private readonly AppDbContext _db = db;

    private static readonly SimpleBloomFilter _filter = new(100);

    [HttpPost("load")]
    public async Task<IActionResult> Load()
    {
        var productIds = await _db.Products.AsNoTracking().Select(x => x.Id).ToListAsync();

        foreach (var id in productIds)
        {
            _filter.Add(id);
        }

        return Ok(new
        {
            message = "Bloom Filter loaded.",
            count = productIds.Count
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        // Bloom Filter check
        if (!_filter.MightContain(id))
        {
            return NotFound(new
            {
                message = "Product definitely does not exist."
            });
        }

        // Bloom Filter says "possibly exists".
        // Now check the real source of truth.
        var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (product == null)
        {
            return NotFound(new
            {
                message = "Bloom Filter said possibly exists, but product was not found in database."
            });
        }

        return Ok(product);
    }
}
