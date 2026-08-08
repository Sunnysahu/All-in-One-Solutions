using GlobalExceptionAndLogging.Entities;

namespace GlobalExceptionAndLogging.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
}