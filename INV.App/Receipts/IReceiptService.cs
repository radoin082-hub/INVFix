using INV.Domain.Entities.Receipts;
using INV.Domain.Shared;

namespace INV.App.Receipts
{
    public interface IReceiptService
    {
        ValueTask<Result<ReceiptDetail>> CreateReceiptFromPurchase(Guid purchaseId);

        ValueTask<Result> ValidateReceipt(Guid receiptId);

        ValueTask<Result<List<ReceiptInfo>>> GetAllReceipts();

        ValueTask<Result<Receipt?>> GetReceiptById(Guid id);

        ValueTask<Result<List<Receipt>>> GetReceiptsByPurchaseId(Guid purchaseId);

        ValueTask<Result> CreateReceipt(Receipt receipt);

        ValueTask<Result> UpdateReceipt(Receipt receipt);

        ValueTask<Result> RemoveReceipt(Guid id);

        ValueTask<Result<List<ReceiptProduct>>> GetAllReceiptProducts();

        ValueTask<Result<List<ReceiptProduct>>> GetProductsByReceptionId(Guid receptionId);

        ValueTask<Result> RemoveReceiptProductAsync(Guid receptionId, Guid productId);

        ValueTask<Result<ReceiptDetail>> GetReceiptInfoById(Guid receiptId);

        ValueTask<Result<List<ReceiptInfo>>> GetReceiptsBySupplierId(Guid supplierId);

        ValueTask<Result<List<ReceiptInfo>>> GetReceiptsByPurchaseIdWhenStatus(Guid purchaseId);

        ValueTask<Result<bool>> ReceiptExistById(Guid id);

        ValueTask<int> GetReceptionCount();

        ValueTask<List<int>> GetReceptionStatusCount();

        ValueTask<long> GetNextReceptionNumber();
        ValueTask<long> UpdateReceptionNumber(Guid purchaseOrderId, long newNumber);
        ValueTask<Result<List<ReceiptDetail>>> GetReceiptDetail(Guid purchaseId);

    }
}