using Redis.Caching;
using Redis.Models;
using Redis.Repository;

namespace Redis.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly IRedisCacheService _cache;

        public ProductService(IProductRepository repo, IRedisCacheService cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task<List<Product>?> GetAll()
        {
            var cache = await _cache.GetAsync<List<Product>>("products");

            if (cache != null) return cache;

            var data = await _repo.GetAll();

            await _cache.SetAsync("products", data);
            return data;
        }
        public async Task<Product?> GetById(int id)
        {
            string key = $"product:{id}";

            var cache = await _cache.GetAsync<Product>(key);

            if (cache != null) return cache;

            var data = await _repo.GetById(id);

            if (data == null) return null;

            await _cache.SetAsync(key, data);
            return data;
        }
        public async Task<Product?> Create(Product product)
        {
            var data = await _repo.Create(product);

            await _cache.RemoveAsync("products");

            return data;
        }

        public async Task<bool> Update(Product product)
        {
            var result = await _repo.Update(product);

            await _cache.RemoveAsync("products");
            await _cache.RemoveAsync($"product:{product.Id}");

            return result;
        }

        public async Task<bool> Delete(int id)
        {
           var result = await _repo.Delete(id);
           if(result == true)
           {
            await _cache.RemoveAsync("products");
            await _cache.RemoveAsync($"product:{id}");
                return result;
           }
            return false;
        }
    }
}
