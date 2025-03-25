using INV.App.Receipts;
using INV.Domain.Entities.Purchases;
using INV.Domain.Entities.Receipts;

namespace INV.Infrastructure.Storage.Receipts;

public interface IReceiptStorage
{
    ValueTask<ReceiptDetail> CreateReceiptFromPurchase(Guid purchaseId);

    ValueTask<List<ReceiptInfo>> SelectAllReceipts();

    ValueTask<Receipt?> SelectReceiptById(Guid id);

    ValueTask<List<Receipt>> SelectReceiptsByPurchaseId(Guid purchaseId);

    ValueTask<int> InsertReceipt(Receipt receipt);

    ValueTask<int> UpdateReceipt(Receipt receipt);

    ValueTask<int> DeleteReceipt(Guid id);

    ValueTask<List<ReceiptProduct>> SelectAllReceiptProducts();

    ValueTask<List<ReceiptProduct>> SelectProductsByReceptionId(Guid receptionId);

    ValueTask<int> InsertReceiptProduct(ReceiptProduct receiptProduct);

    ValueTask<int> UpdateReceiptProduct(ReceiptProduct receiptProduct);

    ValueTask<int> DeleteReceiptProduct(Guid receptionId, Guid productId);

    ValueTask<ReceiptDetail> GetReceiptInfoById(Guid receiptId, bool includeProducts = false);

    ValueTask<string> ValidateReceipt(Guid receiptId);

    ValueTask<List<ReceiptInfo>> SelectReceiptsBySupplierId(Guid supplierId);

    ValueTask<List<ReceiptInfo>> SelectReceiptsByPurchaseIdWhenStatus1(Guid purchaseId);

    ValueTask<bool> ReceiptExistById(Guid id);
}