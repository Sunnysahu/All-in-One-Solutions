using Dependency_Injection_with_DB.Data;
using Dependency_Injection_with_DB.Models;
using Microsoft.EntityFrameworkCore;

namespace Dependency_Injection_with_DB.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }
    }
}
