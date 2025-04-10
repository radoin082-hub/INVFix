using INV.App.Purchases;
using INV.Shared;
using Microsoft.AspNetCore.Components;

namespace INV.Web.Components.Pages.Purchases
{
    public partial class PurchseOrderListPage : ComponentBase
    {

        private string SearchTerm { get; set; } = "";

        private List<PurchaseOrderInfo> displayedItems =>
            purchaseOrderInfos.Where(i =>
                i.Number.ToString().Contains(SearchTerm) ||
                i.SupplierName.ToString().ToLower().Contains(SearchTerm.ToLower()))
                    .ToList();
        [Inject] public NavigationManager navigationManager { set; get; }
        [Inject] public IPurchaseOrderService purchaseOrderService { set; get; }
        private List<PurchaseOrderInfo> purchaseOrderInfos;

        protected override async Task OnInitializedAsync()
        {
            var result= await purchaseOrderService.GetPurchaseOrderInfo();
            if (result.IsSuccess)
            {
                purchaseOrderInfos = result.Value;
            }
            purchaseOrderInfos = purchaseOrderInfos.OrderBy(s => s.Date).ToList();
        }
        private void NavigateToPurchaseOrder() => navigationManager.NavigateTo($"{PageRoutes.CreatePurchase}");
    }
}