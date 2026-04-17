using Dependency_Injection_with_DB.Models;
using Dependency_Injection_with_DB.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Dependency_Injection_with_DB.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly ILogger<ProductService> _logger;


        public ProductService(IProductRepository repo, ILogger<ProductService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _repo.GetAllAsync(); 
        }

    }
}
