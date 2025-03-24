using INV.App.Purchases;
using INV.App.Receipts;
using INV.Domain.Entities.Purchases;
using INV.Domain.Entities.Receipts;
using INV.Domain.Shared;
using INVUIs.Receptions.Models;
using INVUIs.Shared.Models;
using INVUIs.Shared.MyAlert;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace INVUIs.Receptions
{
    public partial class NewReception
    {
        [Parameter] public ReceiptDetail ReceiptInfo { get; set; }
        [Inject] public IPurchaseOrderService PurchaseOrderService { get; set; }
        [Inject] public IReceiptService receptionService { get; set; }
        [Inject] private IJSRuntime jsRuntime { set; get; }
        private MyAlert? myAlert;
        private List<ReceiptProductModel> products { get; set; }
        private bool statusInput = false;
        private bool restVisibility = true;

        protected override async Task OnInitializedAsync()
        {
            myAlert = new MyAlert(jsRuntime);
            if (ReceiptInfo != null && ReceiptInfo.ReceiptProducts != null)
            {
                if (ReceiptInfo.Status == ReceiptStatus.validated)
                {
                    cancelEditing();
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
        }

        

        private Task Validate()
        {
            statusInput = true;
          
            receptionService.ValidateReceipt(ReceiptInfo.Id);
            ReceiptInfo.Status = ReceiptStatus.validated;
            restVisibility = false;
            StateHasChanged();
            return Task.CompletedTask;
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
                foreach (var product in products)
                {
                    var receiptProduct =
                        ReceiptInfo.ReceiptProducts.FirstOrDefault(p => p.ProductId == product.ProductId);
                    if (receiptProduct == null || product.Received <= receiptProduct.Received) continue;
                    await myAlert!.ShowErrorAlert("Error",
                        "The received quantity cannot be greater than the quantity ordered.", MyAlertType.error);
                    return;
                }

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
                        Quantity = products.FirstOrDefault(pp => p.ProductId == pp.ProductId)!.Received,
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

                cancelEditing();
            }
        }

        private void cancelEditing() => statusInput = true;

        private bool checkInputs()
        {
            if (ReceiptInfo.DeliveryDate is null || ReceiptInfo.DeliveryNumber is null)
            {
                return false;
            }

            return true;
        }
    }
}