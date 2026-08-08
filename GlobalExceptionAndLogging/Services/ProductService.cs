using GlobalExceptionAndLogging.Entities;
using GlobalExceptionAndLogging.Exceptions;
using GlobalExceptionAndLogging.Repositories;

namespace GlobalExceptionAndLogging.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        var  product = await _repository.GetByIdAsync(id);

        if (product is null)
        {
            throw new NotFoundException($"Product with ID {id} was not found.");
        }

        return product;
    }
}