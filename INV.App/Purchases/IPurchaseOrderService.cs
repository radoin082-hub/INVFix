using INV.Domain.Entities.Purchases;
using INV.Domain.Shared;

namespace INV.App.Purchases
{
    public interface IPurchaseOrderService
    {
        ValueTask<Result> CreateProductPurchase(PurchaseProduct purchaseProduct);

        ValueTask<Result<List<PurchaseOrder>>> GetPurchaseOrdersByDate(DateOnly dateOnly);

        ValueTask<Result<List<PurchaseOrderInfo>>> GetPurchaseOrderInfo();

        ValueTask<Result<List<PurchaseOrderInfo>>> GetPurchaseOrdersByIdSupplier(Guid idSupplier);

        ValueTask<Result<PurchaseOrder>> GetPurchaseOrdersById(Guid id);

        ValueTask<Result> CreatePurchaseOrder(PurchaseOrder purchaseOrder, List<PurchaseProduct> products);

        ValueTask<Result<List<PurchaseOrderInfo>>> GetPurchasesForReceiptCreation();

        ValueTask<Result<List<PurchaseProductInfo>>> GetProductsByPurchaseId(Guid purchaseId);

        ValueTask<Result> RemovePurchaseProduct(Guid productId, Guid purchaseId);

        ValueTask<Result> DeleteAllPurchaseProduct(Guid purchaseOrderId);

        ValueTask<Result> UpdatePurchaseProduct(PurchaseProduct purchaseProduct);

        ValueTask<Result> UpdatePurchaseOrder(PurchaseOrder purchaseOrder);

        ValueTask<Result<PurchaseStatus>> GetPurchaseStatus(Guid id);

        ValueTask DecisionCF(Guid purchaseId, PurchaseStatus status, DateOnly? date, string? visaNumer, string? motif);

        ValueTask<int> GetPurchaseCount();

        ValueTask<List<int>> GetPurchaseCountsByStatus();

        ValueTask<long> GetNextPurchaseOrderNumberAsync();

        ValueTask<long> UpdatePurchaseOrderNumberAsync(Guid purchaseOrderId, long newNumber);
        ValueTask<Result<List<PurchaseDetail>>> GetPurchaseOrderDetail(Guid purchaseId);

    }
}