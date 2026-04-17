using Dependency_Injection_with_DB.Models;

namespace Dependency_Injection_with_DB.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
    }
}
