using INV.App.Purchases;
using INV.App.Receipts;
using INV.Domain.Entities.Purchases;
using INV.Domain.Entities.Receipts;
using INV.Domain.Shared;
using INVUIs.Receptions.Models;
using INVUIs.Shared.Models;
using INVUIs.Shared.MyAlert;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace INVUIs.Receptions
{
    public partial class NewReception
    {
        [Parameter] public ReceiptDetail ReceiptInfo { get; set; }
        [Inject] public IPurchaseOrderService PurchaseOrderService { get; set; }
        [Inject] public IReceiptService receptionService { get; set; }
        [Inject] private IJSRuntime jsRuntime { set; get; }
        [Inject] private NavigationManager navigationManager { get; set; }
        [Inject] private NavigationLock navigationLock { get; set; }
        private MyAlert? myAlert;
        private List<ReceiptProductModel> products { get; set; }
        private bool statusInput = false;
        private bool restVisibility = true;
        private bool isValidated = false;

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
                    NEwReceived = p.Received,
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


        private async Task Validate()
        {
            statusInput = true;
            isValidated = true;
            var result = await receptionService.ValidateReceipt(ReceiptInfo.Id);
            ReceiptInfo.Status = ReceiptStatus.validated;
            restVisibility = false;
            StateHasChanged();
            navigationManager.NavigateTo("/receptions");
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
                if (products.FindAll(s => s.NEwReceived == 0).Count > 0)
                {
                    await myAlert!.ShowAlert("Error",
                        "The received quantity cannot be zero.", MyAlertType.error);
                    return;
                }

                foreach (var product in products)
                {
                    var receiptProduct =
                        ReceiptInfo.ReceiptProducts.FirstOrDefault(p => p.ProductId == product.ProductId);
                    if (receiptProduct == null || product.NEwReceived <= receiptProduct.Received ||
                        product.NEwReceived == 0) continue;
                    await myAlert!.ShowAlert("Error",
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
                        Quantity = products.FirstOrDefault(pp => p.ProductId == pp.ProductId)!.NEwReceived,
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

        private async Task beforeNavigation(LocationChangingContext context)
        {
            if (!isValidated)
            {
                context.PreventNavigation();
                await myAlert.ShowToast("Error", "You must validate the reception before leaving.", MyAlertType.error);
            }
        }
    }
}