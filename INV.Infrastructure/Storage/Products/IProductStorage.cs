using INV.App.Products;
using INV.Domain.Entities.Products;

namespace INV.Infrastructure.Storage.Products
{
    public interface IProductStorage
    {
        Task<int> InsertProduct(Product product);

        Task<int> UpdateProduct(Product product);

        Task<int> DeleteProduct(Guid id);

        Task<List<ProductInfo>> SelectProducts();

        Task<bool> ProductExistsByaDesignation(string designation);

       // ValueTask<ProductInfo> GetProductById(Guid productId, bool getReceipts = true);
        ValueTask<ProductDetail> GetProductById(Guid productId);
        ValueTask<bool> SelectPurchaseCountByProductId(Guid productId);
    }
}