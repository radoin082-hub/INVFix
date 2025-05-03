using System.Linq;
using INV.App.Purchases;
using INV.App.Receipts;
using INV.App.WareHouses;
using INV.Domain.Entities.Purchases;
using INV.Domain.Entities.Receipts;
using INV.Domain.Entities.WareHouses;
using INV.Domain.Shared;
using INVUIs.Receptions.Models;
using INVUIs.Shared.Models;
using INVUIs.Shared.MyAlert;
using INVUIs.WareHouses.Models;
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
        [Inject] private IWareHouseService wareHouseService { get; set; }
        private ReceptionStorageLocation receptionStorageLocation;
        private MyAlert? myAlert;
        private List<ReceiptProductModel> products { get; set; }
        private List<ReceiptProductInfo> ReceiptProductInfo { get; set; } = new();
        private ReceiptProductModel? product = null;
        public List<WareHouse> WareHouses;
        private bool statusInput = false;
        private bool restVisibility = true;
        private bool isValidated = false;

        public async Task showLocal(ReceiptProductModel receiptProductModel)
        {
            product = receiptProductModel;
            await receptionStorageLocation.Show(product);
            StateHasChanged();
        }

        public async Task<ReceiptProductModel> a(ReceiptProductModel receiptProductModel)
        {
            product = receiptProductModel;
            return product;
        }


        protected override async Task OnInitializedAsync()
        {
            var result = await wareHouseService.GetAllWareHousesByWarhouseType();
            WareHouses = result.Value;
            myAlert = new MyAlert(jsRuntime);
            if (ReceiptInfo != null && ReceiptInfo.ReceiptProducts != null)
            {
                if (ReceiptInfo.Status == ReceiptStatus.validated)
                {
                    isValidated = true;
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
            var numberReception = await receptionService.GetNextReceptionNumber();
            await receptionService.UpdateReceptionNumber(ReceiptInfo.Id, numberReception);
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
            bool send = await checkInputs();
            if (!send) return;

            // Check for zero quantity
            /*if (products.All(p => p.NEwReceived <= 0))
            {
                await myAlert!.ShowAlert(Localizer["Error"],
                    "The received quantity cannot be zero.", MyAlertType.error);
                return;
            }*/

            // Check for exceeding ordered quantity
            foreach (var p in products)
            {
                var existing = ReceiptInfo.ReceiptProducts
                    .FirstOrDefault(rp => rp.ProductId == p.ProductId);
                if (existing != null && p.NEwReceived > existing.Quantity)
                {
                    await myAlert!.ShowAlert(Localizer["Error"],
                        "The received quantity cannot be greater than the quantity ordered.", MyAlertType.error);
                    return;
                }
            }

            // Validate delivery info
            if (ReceiptInfo.DeliveryDate == null || string.IsNullOrWhiteSpace(ReceiptInfo.DeliveryNumber))
            {
                await myAlert!.ShowAlert(Localizer["Error"], Localizer["Delivery.Missing"], MyAlertType.error);
                return;
            }

            // Build receipt product list
            var updatedReceiptProducts = products.Select(p => new ReceiptProductInfo
            {
                ReceptionId = ReceiptInfo.Id,
                ProductId = p.ProductId,
                Quantity = p.NEwReceived,
                Received = p.Received,
                Designation = p.Designation,
                UnitPrice = p.UnitPrice,
                DefaultWareHouseId = p.WareHouseId,
                ReceiptProductDetails = p.ReceiptProductDetails?.Select(d => new ReceiptProductDetails
                {
                    ReceiptProductId = ReceiptInfo.Id,
                    ProductId = d.ProductId,
                    WarhouseId = d.WarhouseId,
                    WarhouseName = d.WarhouseName,
                    Quantity = d.Quantity
                }).ToList() ?? new List<ReceiptProductDetails>()
            }).ToList();

            var receiptToSave = new ReceiptInfo
            {
                Id = ReceiptInfo.Id,
                Number = ReceiptInfo.Number,
                Date = ReceiptInfo.Date,
                PurchaseId = ReceiptInfo.PurchaseId,
                purchaseNumber = ReceiptInfo.purchaseNumber,
                PurchaseDate = ReceiptInfo.PurchaseDate,
                Quantity = updatedReceiptProducts.Sum(p => p.Quantity),
                supplierId = ReceiptInfo.supplierId,
                supplierName = ReceiptInfo.supplierName,
                DeliveryNumber = ReceiptInfo.DeliveryNumber,
                DeliveryDate = ReceiptInfo.DeliveryDate,
                Status = ReceiptStatus.editing,
                ReceiptProducts = updatedReceiptProducts
            };
            
            var existingReceipt = await receptionService.GetReceiptById(ReceiptInfo.Id);

            if (existingReceipt.IsSuccess)
            {
                await receptionService.UpdateReceipt(receiptToSave);
            }
            else
            {
                await receptionService.CreateReceipt(receiptToSave);
            }

            cancelEditing();
        }


        private void cancelEditing()
        {
            statusInput = true;
            isValidated = true;
        }

        private async Task<bool> checkInputs()
        {
            if (ReceiptInfo.DeliveryDate is null || ReceiptInfo.DeliveryNumber is null)
            {
                await myAlert!.ShowAlert(Localizer["Error"], Localizer["Delivery.Missing"]
                    , MyAlertType.error);
                return false;
            }

            return true;
        }

        private async Task beforeNavigation(LocationChangingContext context)
        {
            if (!isValidated)
            {
                context.PreventNavigation();
                await myAlert.ShowToast(null, Localizer["Reception.MustValidate"], MyAlertType.error);
            }
        }

        private void locationsSaved(List<ReceiptProductDetails> details)
        {
            if (product is not null)
            {
                product.ReceiptProductDetails = details;
            }

            StateHasChanged();
        }
    }
}