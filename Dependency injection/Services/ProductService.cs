using Dependency_injection.Models;
using Dependency_injection.Repositories;

namespace Dependency_injection.Services
{
    public class ProductService : IProductService
    {
        public readonly IProductRepository _repo;
        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public void CreateProduct(Product product)
        {
            _repo.Add(product);
        }

        public Product GetProduct(int id)
        {
            return _repo.GetById(id);
        }

        public IEnumerable<Product> GetProducts()
        {
            return _repo.GetAll();
        }
    }
}
