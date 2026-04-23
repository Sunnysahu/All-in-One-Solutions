using Microsoft.EntityFrameworkCore;
using Redis.Data;
using Redis.Models;

namespace Redis.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) => _context = context;
        public async Task<List<Product>?> GetAll()
        {
            return await _context.Products.ToListAsync();
        }
        public async Task<Product?> GetById(int id)
        {
            var result = await _context.Products.FindAsync(id);
            if (result == null) 
            {
                return null;
            }
            return result;
        }

        public async Task<Product?> Create(Product product)
        {
            var result = _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;

        }

        public async Task<bool> Update(Product product)
        {
            _context.Products.Update(product);
            var rows = await _context.SaveChangesAsync();

            return rows > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var result = await _context.Products.FindAsync(id);
            if (result == null)
            {
                return false;
            }

            _context.Products.Remove(result);
            await _context.SaveChangesAsync();
            return true;

            //public async Task<bool> Delete(int id)
            //{
            //    var rows = await _context.Products
            //        .Where(p => p.Id == id)
            //        .ExecuteDeleteAsync();

            //    return rows > 0; // ✅ this is your “==”
            //}
    }


    }
}
