using INV.App.Budgets;
using INV.App.Purchases;
using INV.App.Receipts;
using INV.Domain.Entities.Budget;
using INV.Domain.Entities.Products;
using INV.Domain.Entities.Purchases;
using INV.Domain.Entities.Receipts;
using INV.Domain.Shared;
using INV.Implementation.Service.Purchses;
using INVUIs.Products.ProductsModel;
using INVUIs.Purchases;
using INVUIs.Purchases.PurchaseModels;
using Microsoft.AspNetCore.Components;

namespace INV.Web.Components.Pages.Purchases
{
    public partial class PurchaseDetailPage
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] public IPurchaseOrderService purchaseOrderService { set; get; }
        [Inject] public IReceiptService receiptService { set; get; }

        [Inject] public IBudgetService budgetService { get; set; }

        public PurchaseOrder purchaseOrder = new PurchaseOrder();

        public List<PurchaseProductModel> products = new List<PurchaseProductModel>();

        public List<ReceiptInfo> receptionsListByPurchase;

        private PurchaseHeader purchaseHeaderRef;
        public PurchaseModel purchaseModel { set; get; } = new();

        public List<Article> articles;
        public List<Chapter> chapters;

        protected override async Task OnInitializedAsync()
        {
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
                    SupplierId = purchaseOrder.SupplierId
                    
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
                    UnitMeasure = "U"
                }).ToList();
            }
            var receiptsByPurchase = await receiptService.GetReceiptsByPurchaseIdWhenStatus(purchaseOrder.Id);
            if (receiptsByPurchase.IsSuccess)
            {
                receptionsListByPurchase = receiptsByPurchase.Value.ToList();
            }
        }
    }
}