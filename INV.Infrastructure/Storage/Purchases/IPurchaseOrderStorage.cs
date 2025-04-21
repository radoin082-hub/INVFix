using INV.App.Purchases;
using INV.Domain.Entities.Products;
using INV.Domain.Entities.Purchases;

namespace INV.Infrastructure.Storage.Purchases
{
    public interface IPurchaseOrderStorage
    {
        Task<int> InsertPurchaseOrder(PurchaseOrder purchaseOrder);

        Task<List<PurchaseOrder>> SelectPurchaseOrdersByDate(DateOnly dateOnly);

        IAsyncEnumerable<PurchaseOrderInfo> SelectPurchaseOrderInfo();

        Task<int> InsertPurchaseProduct(PurchaseProduct orderDetail);

        Task<List<PurchaseProduct>> SelectAllPurchaseProduct();

        Task<List<PurchaseOrderInfo>> SelectPurchaseOrdersByIdSupplier(Guid IDSupplier);

        Task<PurchaseOrder> SelectPurchaseOrdersByID(Guid id);

        ValueTask<List<PurchaseOrderInfo>> SelectPurchasesForReceiptCreation();

        ValueTask<List<PurchaseProductInfo>> SelectProductsByPurchaseId(Guid purchaseId); //DR

        //new
        ValueTask<int> DeletePurchaseProduct(Guid productId, Guid purchaseId);

        ValueTask<int> SetPurchaseOrder(PurchaseOrder purchaseOrder);

        ValueTask<int> SetPurchaseProduct(PurchaseProduct purchaseProduct);

        ValueTask<int> DeleteAllPurchaseProduct(Guid purchaseOrderId);

        ValueTask<PurchaseStatus> selectPurchaseStatus(Guid id);

        ValueTask InsertProductPurchase(PurchaseProduct purchaseProduct);

        ValueTask DecinsonCF(Guid purchaseId, PurchaseStatus status, DateOnly? date, string? visaNumber, string? motif);

        ValueTask<int> SelectPurchaseCount();

        ValueTask<List<int>> SelectPurchaseCountsByStatus();

        ValueTask<long> SelectNextPurchaseOrderNumberAsync();

        ValueTask<long> SetPurchaseOrderNumberAsync(Guid purchaseOrderId, long newNumber);
        IAsyncEnumerable<PurchaseDetail> SelectPurchaseDetail(Guid purchaseId);

    }
}