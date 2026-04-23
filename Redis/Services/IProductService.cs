using Redis.Models;

namespace Redis.Services
{
    public interface IProductService
    {
        Task<List<Product>?> GetAll();
        Task<Product?> GetById(int id);
        Task<Product?> Create(Product product);
        Task<bool> Update(Product product);
        Task<bool> Delete(int id);
    }
}
