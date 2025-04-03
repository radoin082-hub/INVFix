using System.Runtime.InteropServices.JavaScript;
using INV.App.Products;
using INV.Domain.Entities.Products;
using INV.Domain.Shared;
using INV.Infrastructure.Storage.Products;
using INV.Infrastructure.Storage.SupplierStorages;

namespace INV.Implementation.Service.Products
{
    public class ProductService : IProductService
    {
        private IProductStorage productStorage;

        public ProductService(IProductStorage productStorage)
        {
            this.productStorage = productStorage;
        }

        public async ValueTask<Result> SetProducts(Product product)
        {
            try
            {
                await productStorage.UpdateProduct(product);
                return Result.Success();
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask<Result> RemoveProduct(Guid id)
        {
            try
            {
                bool purchaseCount = await productStorage.SelectPurchaseCountByProductId(id);
                if (purchaseCount)
                    return Error.Failure("ErrorDelete", "The product has purchases.");
                await productStorage.DeleteProduct(id);
                return Result.Success();
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask<Result<List<ProductInfo>>> GetProducts()
        {
            try
            {
                var result = await productStorage.SelectProducts();
                return Result.Success(result);
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask<Result<ProductDetail>> GetProductById(Guid id)
        {
            try
            {
                var result = await productStorage.GetProductById(id);
                return Result.Success(result);
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask<Result> CreateProduct(Product product)
        {
            try
            {
                List<Error> errorList = validateProductCreate(product);
                if (errorList.Any())
                    return Result.Failure(errorList.First());

                bool designationExists = await productStorage.ProductExistsByaDesignation(product.Designation);
                if (designationExists)
                    errorList.Add(Error.Conflict("Product.DesignationExists", $"The designation '{product.Designation}' already exists."));

                await productStorage.InsertProduct(product);
                return Result.Success();
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        private static List<Error> validateProductCreate(Product product)
        {
            List<Error> errors = new List<Error>();

            if (string.IsNullOrWhiteSpace(product.Designation))
                errors.Add(ProductError.DesignationExsist);

            return errors;
        }

        public async ValueTask<int> GetProductCountAsync()
        {
            return await productStorage.SelectProductCount();
        }
    }
}