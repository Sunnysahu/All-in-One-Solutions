using GlobalExceptionAndLogging.Entities;

namespace GlobalExceptionAndLogging.Services;

public interface IProductService
{
    Task<Product> GetByIdAsync(int id);
}