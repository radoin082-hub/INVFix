using INV.App.Budgets;
using INV.App.Purchases;
using INV.App.Receipts;
using INV.App.Suppliers;
using INV.Domain.Entities.Budget;
using INV.Domain.Entities.Products;
using INV.Domain.Entities.Purchases;
using INV.Domain.Entities.Receipts;
using INV.Domain.Entities.Suppliers;
using INV.Domain.Shared;
using INV.Implementation.Service.Products;
using INV.Implementation.Service.Purchses;
using INVUIs.Products.ProductsModel;
using INVUIs.Purchases;
using INVUIs.Purchases.PurchaseModels;
using INVUIs.Shared;
using INVUIs.Shared.MyAlert;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;
using Xunit.Sdk;

namespace INV.Web.Components.Pages.Purchases
{
    public partial class PurchaseDetailPage :ComponentBase
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] public IPurchaseOrderService purchaseOrderService { set; get; }
        [Inject] public ISupplierService supplierService { set; get; }
        [Inject] public IReceiptService receiptService { set; get; }
        [Inject] public NavigationManager Navigation { get; set; }

        [Inject] public IBudgetService budgetService { get; set; }

        [Inject] private IJSRuntime jsRuntime { set; get; }
        public PurchaseOrder purchaseOrder = new PurchaseOrder();

        public List<PurchaseProductModel> products = new List<PurchaseProductModel>();

        public List<ReceiptInfo> receptionsListByPurchase;

        private PurchaseHeader purchaseHeaderRef;
        private ISupplier supplier;

        private bool canEdit = true;
        private MyAlert? myAlert;
        private string succesMessage;
        public PurchaseModel purchaseModel { set; get; } = new();

        private SupplierInfo selectedSupplier = new();
        private bool displayVisa = false;
        private bool displayReject = false;
        private ConformationForm conformationForm;
        public List<Article> articles;
        public List<Chapter> chapters;

        private void modeEditing()
        {
            canEdit = !canEdit;
            StateHasChanged();
        }

        private async void modify()
        {
            if (canEdit)
            {
                PurchaseOrder purchaseUpdate = new PurchaseOrder()
                {
                    BudgetArticle = purchaseModel.ArticleCode,
                    ServiceType = purchaseModel.selectedService,
                    BudgetType = purchaseModel.selectedCategory,
                    SupplierId = selectedSupplier.ID,
                    CompletionDelay = int.Parse(purchaseModel.DeliveryTime),
                    BudgetChapter = purchaseModel.ChapterCode,
                    Id = purchaseModel.Id,
                    Date = purchaseModel.Date,
                    Status = purchaseModel.Status,
                    Observation = purchaseModel.Observation,
                    VisaDate = purchaseModel.VisaDate,
                    VisaNumber = purchaseModel.VisaNumber,
                    TotalTTC = purchaseModel.TotalTTC,
                    TotalHT = purchaseModel.TotalHT,
                    TotalTVA = purchaseModel.TotalTVA,
                };

                await purchaseOrderService.UpdatePurchaseOrder(purchaseUpdate);
                Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            myAlert = new MyAlert(jsRuntime);

            var articelList = await budgetService.GetAllArticles();
            if (articelList.IsSuccess)
            {
                articles = articelList.Value;
            }

            var cahpterList = await budgetService.GetAllChapitres();
            if (cahpterList.IsSuccess)
            {
                chapters = cahpterList.Value;
            }

            var resultToPurchase2 = await purchaseOrderService.GetPurchaseOrdersById(Id);
            if (resultToPurchase2.IsSuccess)
            {
                var purchaseOrder = resultToPurchase2.Value;
                purchaseModel = new PurchaseModel
                {
                    ArticleCode = purchaseOrder.BudgetArticle,
                    ChapterCode = purchaseOrder.BudgetChapter,
                    selectedCategory = purchaseOrder.BudgetType,
                    selectedService = purchaseOrder.ServiceType,
                    DeliveryTime = purchaseOrder.CompletionDelay.ToString(),
                    SupplierId = purchaseOrder.SupplierId,
                    Id = purchaseOrder.Id,
                    Date = purchaseOrder.Date,
                    Status = purchaseOrder.Status,
                    Observation = purchaseOrder.Observation,
                    VisaDate = purchaseOrder.VisaDate,
                    VisaNumber = purchaseOrder.VisaNumber,
                };
            }

            /*===================*/
            var resultToPurchase = await purchaseOrderService.GetPurchaseOrdersById(Id);
            if (resultToPurchase.IsSuccess)
            {
                purchaseOrder = resultToPurchase.Value;
            }
            var resultToproduct = await purchaseOrderService.GetProductsByPurchaseId(Id);
            if (resultToproduct.IsSuccess)
            {
                products = resultToproduct.Value.Select(s => new PurchaseProductModel()
                {
                    PurchaseOrderId = s.PurchaseId,
                    Id = s.ProductId,
                    Designation = s.Designation,
                    Quantity = s.Quantity,
                    UnitPrice = s.UnitPrice,

                    TVA = s.TVA,
                    TotalPrice = s.Quantity * s.UnitPrice,
                    Received = s.Received
                }).ToList();
            }
            var receiptsByPurchase = await receiptService.GetReceiptsByPurchaseIdWhenStatus(purchaseOrder.Id);
            if (receiptsByPurchase.IsSuccess)
            {
                receptionsListByPurchase = receiptsByPurchase.Value.ToList();
            }

            var result = await supplierService.GetSupplierById(purchaseOrder.SupplierId);
            if (result.IsSuccess)
            {
                supplier = result.Value;
                selectedSupplier = new SupplierInfo
                {
                    ID = supplier.Id,
                    Name = supplier.ManagerName,
                    Address = supplier.Address,
                    Email = supplier.Email,
                    Phone = supplier.Phone,
                    AccountName = supplier.ManagerName,
                    ART = supplier.ART,
                    BankAgency = supplier.BankAgency,
                    CompanyName = supplier.CompanyName,
                    NIF = supplier.NIF,
                    NIS = supplier.NIS,
                    RC = supplier.RC,
                    RIB = supplier.RIB
                };
            }
            else
            {
                Console.WriteLine("Failed to load supplier details.");
            }
        }

        private void showConformation()
        {
            conformationForm.show();
        }

        private async Task ConfirmDeleteProduct()
        {
            await purchaseOrderService.DecisionCF(purchaseModel.Id, PurchaseStatus.Validated, null, null, null);
            var numberPurchase = await purchaseOrderService.GetNextPurchaseOrderNumberAsync();
            await purchaseOrderService.UpdatePurchaseOrderNumberAsync(purchaseModel.Id, numberPurchase);
            conformationForm.hide();
            Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
            await myAlert.ShowToast(succesMessage, Localizer["Purchase.Validated"], MyAlertType.success);
        }

        public void Edit()
        {
            canEdit = !canEdit;
            modify();
        }

        private void visibilityReject()
        {
            displayReject = !displayReject;
            StateHasChanged();
        }

        private void visibilityVisa()
        {
            displayVisa = !displayVisa;
            StateHasChanged();
        }

        private async void updatePurchaseToReject()
        {
            await purchaseOrderService.DecisionCF(purchaseModel.Id, PurchaseStatus.Reject, purchaseModel.VisaDate, purchaseModel.VisaNumber, purchaseModel.Observation);

            visibilityReject();
            Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
            await Task.Delay(100);
            await myAlert.ShowToast(succesMessage, Localizer["Purchase.Rejected"], MyAlertType.success);
            StateHasChanged();
        }

        private async void updatePurchaseToVisa()
        {
            await purchaseOrderService.DecisionCF(purchaseModel.Id, PurchaseStatus.Vised, purchaseModel.VisaDate, purchaseModel.VisaNumber, null);
            visibilityVisa();
            Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
            await Task.Delay(100);
            await myAlert.ShowToast(succesMessage, Localizer["Purchase.Validated"], MyAlertType.success);

            StateHasChanged();
        }
    }
}