using Dependency_injection.Data;
using Dependency_injection.Models;

namespace Dependency_injection.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context; 
        }

        public IEnumerable<Product> GetAll()
        {
            return _context.Products.ToList();
        }

        public Product GetById(int id)
        {
            return _context.Products.Find(id);
        }
        public void Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

    }
}