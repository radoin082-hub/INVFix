using INV.App.Budgets;
using INV.App.Purchases;
using INV.App.Suppliers;
using INV.Domain.Entities.Budget;
using INV.Domain.Entities.Purchases;
using INVUIs.Products.ProductsModel;
using INVUIs.Purchases;
using INVUIs.Purchases.PurchaseModels;
using INVUIs.Shared.MyAlert;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace INV.Web.Components.Pages.Purchases
{
    public partial class NewPurchasePage
    {
        [Inject] public IPurchaseOrderService purchaseOrderService { get; set; }
        [Inject] public NavigationManager navigationManager { set; get; }
        [Inject] private IBudgetService budgetService { get; set; }
        [Inject] private IJSRuntime jsRuntime { set; get; }
        private readonly List<PurchaseProductModel> productModel = new();
        private bool showAlert = false;
        private MyAlert? myAlert;
        public PurchaseModel purchaseModel { set; get; } = new();
        private SupplierInfo selectedSupplier = new();
        private PurchaseHeader purchaseHeaderRef;
        private List<Chapter> chapters;
        private List<Article> articles;
        private string errorMessage { get; set; }
        private string succesMessage;

        protected override async Task OnInitializedAsync()
        {
            myAlert = new MyAlert(jsRuntime);

            var result2 = await budgetService.GetAllChapitres();
            if (result2.IsSuccess)
            {
                chapters = result2.Value;
            }
        }

        private async Task create()
        {
            await purchaseHeaderRef.SubmitForm();
            if (productModel.Count == 0)
            {
                showError("Please add at least one product before submitting the order.");
                return;
            }

            if (selectedSupplier.Name is null)
            {
                showError("Please select a supplier.");
                return;
            }

            purchaseModel.ProductModels = productModel;

            var purchaseOrder = new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                SupplierId = selectedSupplier.ID,
                BudgetArticle = purchaseModel.ArticleCode,
                BudgetChapter = purchaseModel.ChapterCode,
                BudgetType = purchaseModel.selectedCategory,
                ServiceType = purchaseModel.selectedService,
                CompletionDelay = int.Parse(purchaseModel.DeliveryTime),
                Date = DateOnly.FromDateTime(DateTime.Now.Date),
                TotalHT = purchaseModel.TotalHT,
                TotalTVA = purchaseModel.TotalTVA,
                TotalTTC = purchaseModel.TotalTTC
            };

            var products = productModel.Select(pd => new PurchaseProduct
            {
                PurchaseOrderId = purchaseOrder.Id,
                ProductId = pd.Id,
                UnitPrice = pd.UnitPrice,
                Quantity = pd.Quantity
            }).ToList();
            var result = await purchaseOrderService.CreatePurchaseOrder(purchaseOrder, products);
            if (result.IsSuccess)
            {
                navigationManager.NavigateTo("/purchases");
                await myAlert.ShowToast(succesMessage, Localizer["Purchase.Created"], MyAlertType.success);
            }
        }

        private async void showError(string message)
        {
            errorMessage = message;
            showAlert = true;
            await Task.Delay(2500);
            showAlert = false;
            errorMessage = "";
            StateHasChanged();
        }
    }
}