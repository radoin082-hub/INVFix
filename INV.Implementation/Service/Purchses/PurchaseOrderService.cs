using System.Transactions;
using INV.App.Purchases;
using INV.Domain.Entities.Purchases;
using INV.Domain.Shared;
using INV.Infrastructure.Storage.Products;
using INV.Infrastructure.Storage.Purchases;

namespace INV.Implementation.Service.Purchses
{
    public class PurchaseOrderService(IPurchaseOrderStorage purchaseOrderStorage, IProductStorage productStorage)
        : IPurchaseOrderService
    {
        public async ValueTask<Result<List<PurchaseOrder>>> GetPurchaseOrdersByDate(DateOnly dateOnly)
        {
            try
            {
                var result = await purchaseOrderStorage.SelectPurchaseOrdersByDate(dateOnly);
                return Result.Success(result);
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask<Result<List<PurchaseOrderInfo>>> GetPurchaseOrderInfo()
        {
            try
            {
                IAsyncEnumerable<PurchaseOrderInfo> result = purchaseOrderStorage.SelectPurchaseOrderInfo();
                return Result.Success(await result.ToListAsync());
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask<Result<List<PurchaseOrderInfo>>> GetPurchaseOrdersByIdSupplier(Guid idSupplier)
        {
            try
            {
                var result = await purchaseOrderStorage.SelectPurchaseOrdersByIdSupplier(idSupplier);

                return Result.Success(result);
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask<Result<PurchaseOrder>> GetPurchaseOrdersById(Guid id)
        {
            try
            {
                var result = await purchaseOrderStorage.SelectPurchaseOrdersByID(id);
                return Result.Success(result);
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask<Result> CreatePurchaseOrder(PurchaseOrder purchaseOrder, List<PurchaseProduct> products)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    await purchaseOrderStorage.InsertPurchaseOrder(purchaseOrder);

                    foreach (var product in products) await purchaseOrderStorage.InsertProductPurchase(product);

                    scope.Complete();
                    return Result.Success();
                }
                catch (Exception ex)
                {
                    return Error.Exception(ex);
                }
            }
        }

        public async ValueTask<Result> CreateProductPurchase(PurchaseProduct purchaseProduct)
        {
            try
            {
                await purchaseOrderStorage.InsertProductPurchase(purchaseProduct);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Error.Exception(ex);
            }
        }

        public async ValueTask<Result<List<PurchaseOrderInfo>>> GetPurchasesForReceiptCreation()
        {
            try
            {
                var result = await purchaseOrderStorage.SelectPurchasesForReceiptCreation();
                return Result.Success(result);
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask<Result<List<PurchaseProductInfo>>> GetProductsByPurchaseId(Guid purchaseId)
        {
            try
            {
                var result = await purchaseOrderStorage.SelectProductsByPurchaseId(purchaseId);
                return Result.Success(result);
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask<Result> RemovePurchaseProduct(Guid productId, Guid purchaseId)
        {
            try
            {
                await purchaseOrderStorage.DeletePurchaseProduct(productId, purchaseId);
                return Result.Success();
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask<Result> UpdatePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            try
            {
                await purchaseOrderStorage.SetPurchaseOrder(purchaseOrder);
                return Result.Success();
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask<Result> UpdatePurchaseProduct(PurchaseProduct purchaseProduct)
        {
            try
            {
                await purchaseOrderStorage.SetPurchaseProduct(purchaseProduct);
                return Result.Success();
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask<Result> DeleteAllPurchaseProduct(Guid purchaseOrderId)
        {
            try
            {
                await purchaseOrderStorage.DeleteAllPurchaseProduct(purchaseOrderId);
                return Result.Success();
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask<Result<PurchaseStatus>> GetPurchaseStatus(Guid id)
        {
            try
            {
                var result = await purchaseOrderStorage.selectPurchaseStatus(id);
                return Result.Success(result);
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask DecisionCF(Guid purchaseId, PurchaseStatus status, DateOnly? date, string visaNumber, string motif)
        {
            await purchaseOrderStorage.DecinsonCF(purchaseId, status, date, visaNumber, motif);
        }

        public async ValueTask<int> GetPurchaseCount()
        {
            return await purchaseOrderStorage.SelectPurchaseCount();
        }

        public async ValueTask<List<int>> GetPurchaseCountsByStatus()
        {
            return await purchaseOrderStorage.SelectPurchaseCountsByStatus();
        }
    }
}