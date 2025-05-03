using System.Transactions;
using INV.App.Receipts;
using INV.Domain.Entities.Receipts;
using INV.Domain.Shared;
using INV.Infrastructure.Storage.Receipts;

namespace INV.Implementation.Service.Receipts
{
    public class ReceiptService(IReceiptStorage receiptStorage) : IReceiptService
    {
        public async ValueTask<Result<ReceiptDetail>> CreateReceiptFromPurchase(Guid purchaseId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var receipt = await receiptStorage.CreateReceiptFromPurchase(purchaseId);
                    scope.Complete();
                    return Result.Success(receipt);
                }
                catch (Exception ex)
                {
                    return Error.Exception(ex);
                }
            }
        }

        public async ValueTask<Result> ValidateReceipt(Guid receiptId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    await receiptStorage.ValidateReceipt(receiptId);
                    scope.Complete();
                    return Result.Success();
                }
                catch (Exception ex)
                {
                    return Error.Exception(ex);
                }
            }
        }

        public async ValueTask<Result<List<ReceiptInfo>>> GetAllReceipts()
        {
            try
            {
                var receipts = await receiptStorage.SelectAllReceipts();
                return receipts;
            }
            catch (Exception ex)
            {
                return Error.Exception(ex);
            }
        }

        public async ValueTask<Result<Receipt?>> GetReceiptById(Guid id)
        {
            try
            {
                var receipt = await receiptStorage.SelectReceiptById(id);

                return receipt is null ? ReceiptError.ReceiptNotFound(id) : receipt;
            }
            catch (Exception ex)
            {
                return Error.Exception(ex);
            }
        }

        public async ValueTask<Result<List<Receipt>>> GetReceiptsByPurchaseId(Guid purchaseId)
        {
            try
            {
                var receipts = await receiptStorage.SelectReceiptsByPurchaseId(purchaseId);
                return receipts;
            }
            catch (Exception ex)
            {
                return Error.Exception(ex);
            }
        }

        public async ValueTask<Result> CreateReceipt(ReceiptInfo receipt)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    /*await receiptStorage.InsertReceipt(receipt);
                    foreach (var product in receipt.Products)
                    {
                        await receiptStorage.InsertReceiptProduct(product);
                    }*/
                    var entity = MapToReceipt(receipt);
                    await receiptStorage.InsertReceipt(entity);
                    foreach (var product in receipt.ReceiptProducts)
                    {
                        foreach (var item in product.ReceiptProductDetails)
                        {
                            var mappedProduct = MapToReceiptProduct(item);
                            await receiptStorage.InsertReceiptProduct(mappedProduct);
                        }
                    }

                    scope.Complete();
                    return Result.Success();
                }
                catch (Exception ex)
                {
                    return Error.Exception(ex);
                }
            }
        }

        public async ValueTask<Result> UpdateReceipt(ReceiptInfo receipt)
        {
            using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var entity = MapToReceipt(receipt);
                    await receiptStorage.UpdateReceipt(entity);
                    foreach (var product in receipt.ReceiptProducts)
                    {
                        /*var mappedProduct = MapToReceiptProduct(product);
                        await receiptStorage.UpdateReceiptProduct(mappedProduct);*/
                    }

                    scope.Complete();
                    return Result.Success();
                }
                catch (Exception ex)
                {
                    return Error.Exception(ex);
                }
            }
        }

        public async ValueTask<Result> RemoveReceipt(Guid id)
        {
            try
            {
                await receiptStorage.DeleteReceipt(id);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Error.Exception(ex);
            }
        }

        public async ValueTask<Result<List<ReceiptProduct>>> GetAllReceiptProducts()
        {
            try
            {
                var receiptProducts = await receiptStorage.SelectAllReceiptProducts();
                return Result.Success(receiptProducts);
            }
            catch (Exception ex)
            {
                return Error.Exception(ex);
            }
        }

        public async ValueTask<Result<List<ReceiptProduct>>> GetProductsByReceptionId(Guid receptionId)
        {
            try
            {
                var receiptProducts = await receiptStorage.SelectProductsByReceptionId(receptionId);
                return receiptProducts;
            }
            catch (Exception ex)
            {
                return Error.Exception(ex);
            }
        }

        public async ValueTask<Result> RemoveReceiptProductAsync(Guid receptionId, Guid productId)
        {
            try
            {
                await receiptStorage.DeleteReceiptProduct(receptionId, productId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Error.Exception(ex);
            }
        }

        public async ValueTask<Result<ReceiptDetail>> GetReceiptInfoById(Guid receiptId)
        {
            try
            {
                var receiptInfo = await receiptStorage.GetReceiptInfoById(receiptId, true);

                return Result.Success(receiptInfo);
            }
            catch (Exception ex)
            {
                return Error.Exception(ex);
            }
        }

        public async ValueTask<Result<List<ReceiptInfo>>> GetReceiptsBySupplierId(Guid supplierId)
        {
            try
            {
                var result = await receiptStorage.SelectReceiptsBySupplierId(supplierId);
                return Result.Success(result);
            }
            catch (Exception ex)
            {
                return Error.Exception(ex);
            }
        }

        public async ValueTask<Result<List<ReceiptInfo>>> GetReceiptsByPurchaseIdWhenStatus(Guid purchaseId)
        {
            try
            {
                var result = await receiptStorage.SelectReceiptsByPurchaseIdWhenStatus1(purchaseId);
                return Result.Success(result);
            }
            catch (Exception ex)
            {
                return Error.Exception(ex);
            }
        }

        public async ValueTask<Result<bool>> ReceiptExistById(Guid id)
        {
            try
            {
                return Result.Success(await receiptStorage.ReceiptExistById(id));
            }
            catch (Exception ex)
            {
                return Error.Exception(ex);
            }
        }

        public async ValueTask<int> GetReceptionCount()
        {
            return await receiptStorage.SelectSupplierCount();
        }

        public async ValueTask<List<int>> GetReceptionStatusCount()
        {
            return await receiptStorage.SelectReceptionCountsByStatus();
        }

        private async Task<List<Error>> ValidateReceiptCreate(Guid purchaseId)
        {
            List<Error> errors = new List<Error>();

            var purchase = await receiptStorage.SelectReceiptsByPurchaseId(purchaseId);
            if (!purchase.Any())
                errors.Add(ReceiptError.ReceiptNotFound(purchaseId));

            var purchaseOrder = await receiptStorage.SelectReceiptById(purchaseId);
            if (purchaseOrder == null || purchaseOrder.Status != ReceiptStatus.validated)
                errors.Add(ReceiptError.InvalidReceiptStatus(ReceiptStatus.editing));

            var existingReceipts = await receiptStorage.SelectReceiptsByPurchaseId(purchaseId);
            if (existingReceipts.Any())
                errors.Add(ReceiptError.ReceiptAlreadyValidated(purchaseId));

            return errors;
        }

        public async ValueTask<long> GetNextReceptionNumber()
        {
            return await receiptStorage.SelectNextReceptionNumber();
        }

        public async ValueTask<long> UpdateReceptionNumber(Guid purchaseOrderId, long newNumber)
        {
            return await receiptStorage.SetReceptionNumber(purchaseOrderId, newNumber);
        }

        public async ValueTask<Result<List<ReceiptDetail>>> GetReceiptDetail(Guid purchaseId)
        {
            try
            {
                IAsyncEnumerable<ReceiptDetail> result = receiptStorage.SelectReceptionDetail(purchaseId);
                return Result.Success(await result.ToListAsync());
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        private Receipt MapToReceipt(ReceiptInfo info)
        {
            return new Receipt
            {
                Id = info.Id,
                PurchaseId = info.PurchaseId,
                Date = info.Date ?? default,
                DeliveryNumber = info.DeliveryNumber,
                Number = info.Number,
                DeliveryDate = info.DeliveryDate ?? default,
                Status = info.Status,
                Products = info.ReceiptProducts.Select(p => new ReceiptProduct
                {
                    ReceptionId = p.ReceptionId,
                    ProductId = p.ProductId,
                    Quantity = p.Quantity,
                }).ToList()
            };
        }

        private ReceiptProduct MapToReceiptProduct(ReceiptProductDetails info)
        {
            return new ReceiptProduct()
            {
                ReceptionId = info.ReceiptProductId,
                ProductId = info.ProductId,
                Quantity = info.Quantity,
                WareHouseId = info.WarhouseId
            };
        }
    }
}