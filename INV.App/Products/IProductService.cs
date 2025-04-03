using INV.Domain.Entities.Products;
using INV.Domain.Shared;

namespace INV.App.Products
{
    public interface IProductService
    {
        ValueTask<Result> CreateProduct(Product product);

        ValueTask<Result> SetProducts(Product product);

        ValueTask<Result> RemoveProduct(Guid id);

        ValueTask<Result<List<ProductInfo>>> GetProducts();

        ValueTask<Result<ProductDetail>> GetProductById(Guid id);

        ValueTask<int> GetProductCountAsync();
    }
}