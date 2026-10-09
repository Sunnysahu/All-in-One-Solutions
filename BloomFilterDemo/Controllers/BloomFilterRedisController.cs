using BloomFilterDemo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace BloomFilterDemo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BloomFilterRedisController(AppDbContext db, IDistributedCache cache) : ControllerBase
{
    private readonly AppDbContext _db = db; 
    private readonly IDistributedCache _cache = cache; 
    private const string FilterKey = "products:bloom-filter"; 
    private const int FilterSize = 10000; 
    private const int ByteSize = (FilterSize + 7) / 8; 

    [HttpPost("load")] 
    public async Task<IActionResult> Load(CancellationToken cancellationToken) 
    { 
        var productIds = await _db.Products.AsNoTracking().Select(x => x.Id).ToListAsync(cancellationToken);
        var bits = new byte[ByteSize]; 

        foreach (var id in productIds) 
        {
            SetBit(bits, GetHash1(id)); 
            SetBit(bits, GetHash2(id)); 
        } 
        
        await _cache.SetAsync(FilterKey, bits, 
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            },
            cancellationToken
        ); 

        return Ok(new { message = "Redis Bloom Filter loaded.", count = productIds.Count }); 
    }

    [HttpGet("{id:int}")] 
    public async Task<IActionResult> GetProduct(int id, CancellationToken cancellationToken) 
    { 
        var bits = await _cache.GetAsync(FilterKey, cancellationToken); 
        
        if (bits == null) 
        { 
            return Conflict(new { message = "Bloom Filter is not loaded in Redis." });
        }

        if (!MightContain(bits, id))
        { 
            return NotFound(new { message = "Product definitely does not exist." });
        }
        
        var product = await _db.Products.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken); 

        if (product == null) 
        { 
            return NotFound(new 
            { 
                message = "Bloom Filter says possibly exists, but product was not found in the database." 
            }); 
        } 
        
        return Ok(product); 
    }

    private static int GetHash1(int id) => (int) (Math.Abs((long) id) % FilterSize); 
    private static int GetHash2(int id) => (int) (Math.Abs((long) id * 31) % FilterSize); 
    private static void SetBit(byte[] bits, int position) { int byteIndex = position / 8; 
    
        int bitIndex = position % 8; 
        
        bits[byteIndex] |= (byte) (1 << bitIndex); 
    }
    private static bool IsBitSet(byte[] bits, int position) 
    { 
        int byteIndex = position / 8; 
        int bitIndex = position % 8; 

        return (bits[byteIndex] & (1 << bitIndex)) != 0;
    }
    private static bool MightContain(byte[] bits, int id) 
    { 
        return IsBitSet(bits, GetHash1(id)) && IsBitSet(bits, GetHash2(id));
    }
}