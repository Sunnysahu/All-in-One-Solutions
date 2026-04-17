using Dependency_injection.Models;

namespace Dependency_injection.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetProducts();
        Product GetProduct(int id);
        void CreateProduct(Product product);
    }
}
