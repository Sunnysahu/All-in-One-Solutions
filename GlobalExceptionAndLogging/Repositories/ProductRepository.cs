using GlobalExceptionAndLogging.Data;
using GlobalExceptionAndLogging.Entities;
using Microsoft.EntityFrameworkCore;

namespace GlobalExceptionAndLogging.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
    }
}