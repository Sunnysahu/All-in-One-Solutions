using Dependency_Injection_with_DB.Models;

namespace Dependency_Injection_with_DB.Models
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();
    }
}
