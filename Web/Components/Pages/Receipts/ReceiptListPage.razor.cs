using INV.App.Purchases;
using INV.App.Receipts;
using INVUIs.Purchases;
using Microsoft.AspNetCore.Components;


namespace INV.Web.Components.Pages.Receipts;

public partial class ReceiptListPage
{
    [Inject] public IPurchaseOrderService purchaseService { get; set; }
    [Inject] public IReceiptService ReceiptService { get; set; }
    [Inject] public NavigationManager navigationManager { set; get; }
    private List<PurchaseOrderInfo> purchases;

    private List<ReceiptInfo> receipts;

    protected override async Task OnInitializedAsync()
    {
        var resultToPurchase = await purchaseService.GetPurchasesForReceiptCreation();
        if (resultToPurchase.IsSuccess)
        {
            purchases = resultToPurchase.Value;
        }
        var resultToReceipt = await ReceiptService.GetAllReceipts();
        if (resultToReceipt.IsSuccess)
        {
            receipts = resultToReceipt.Value;
        }
    }

    private PurchaseSelector purchaseSelector;

    private async Task CommandSelectednew(Guid purchaseId)
    {
        navigationManager.NavigateTo($"/receptions/new/{purchaseId}");
    }
}