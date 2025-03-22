using INV.App.Purchases;
using INV.App.Receipts;
using INV.Domain.Entities.Purchases;
using INV.Domain.Entities.Receipts;
using INV.Domain.Shared;
using INVUIs.Receptions.Models;
using INVUIs.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace INVUIs.Receptions
{
    public partial class NewReception
    {
        [Inject] public IPurchaseOrderService PurchaseOrderService { get; set; }
        [Inject] public IReceiptService receptionService { get; set; }
        [Parameter] public ReceiptDetail ReceiptInfo { get; set; }
        private List<ReceiptProductModel> products { get; set; }
        private bool statusInput = false;
        private bool restVisibility = true;

        protected override async Task OnInitializedAsync()
        {
            if (ReceiptInfo != null && ReceiptInfo.ReceiptProducts != null)
            {
                if (ReceiptInfo.Status == ReceiptStatus.validated)
                {
                    CancelEditing();
                    restVisibility = false;
                }

                products = ReceiptInfo.ReceiptProducts.Select(p => new ReceiptProductModel()
                {
                    ProductId = p.ProductId,
                    UnitPrice = p.UnitPrice,
                    Quantity = p.Quantity,
                    Designation = p.Designation,
                    Received = p.Received
                }).ToList();
            }
            else
            {
                products = new List<ReceiptProductModel>();
            }
        }

        private void Create()
        {
            throw new NotImplementedException();
        }

        private async Task Validate()
        {
            statusInput = true;
            receptionService.ValidateReceipt(ReceiptInfo.Id);
            ReceiptInfo.Status = ReceiptStatus.validated;
            restVisibility = false;
            StateHasChanged();
        }

        private void StartEditing()
        {
            statusInput = false;
        }

        private async Task SaveChanges()
        {
            bool send = checkInputs();
            if (send)
            {
                Receipt receiptToSave = new()
                {
                    Id = ReceiptInfo.Id,
                    Date = (DateOnly)ReceiptInfo.Date,
                    DeliveryDate = (DateOnly)ReceiptInfo.DeliveryDate,
                    DeliveryNumber = ReceiptInfo.DeliveryNumber,
                    PurchaseId = ReceiptInfo.PurchaseId,
                    Products = ReceiptInfo.ReceiptProducts.Select(p => new ReceiptProduct()
                    {
                        ReceptionId = p.ReceptionId,
                        ProductId = p.ProductId,
                        Quantity = products.FirstOrDefault(pp => p.ProductId == pp.ProductId).Received,
                        WareHouseId = p.DefaultWareHouseId
                    }).ToList(),
                    Status = ReceiptStatus.editing
                };

                var result = await receptionService.GetReceiptById(ReceiptInfo.Id);

                if (result.IsSuccess)
                {
                    await receptionService.UpdateReceipt(receiptToSave);
                }
                else
                {
                    await receptionService.CreateReceipt(receiptToSave);
                }

                CancelEditing();
            }
        }

        private void CancelEditing()
        {
            statusInput = true;
        }

        private bool checkInputs()
        {
            if (ReceiptInfo.DeliveryDate == null || ReceiptInfo.DeliveryNumber == null)
            {
                return false;
            }

            return true;
        }
    }
}