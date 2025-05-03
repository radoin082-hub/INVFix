using INV.App.Receipts;
using Microsoft.AspNetCore.Components;

namespace INV.Web.Components.Pages.Receipts;

public partial class ReceiptPage :ComponentBase
{
    [Inject] public IReceiptService receiptService { set; get; }
    [Parameter] public Guid Id { get; set; } = Guid.Empty;
    [Parameter] public Guid purchaseId { get; set; } = Guid.Empty;
    public ReceiptDetail receiptInfo { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Id != Guid.Empty)
        {
            var receipt = await receiptService.GetReceiptInfoById(Id);
            if (receipt.IsSuccess)
            {
                receiptInfo = receipt.Value;
            }
        }
        else
        {
            var receipt = await receiptService.CreateReceiptFromPurchase(purchaseId);
            if (receipt.IsSuccess)
            {
                receiptInfo = receipt.Value;
            }
        }
    }
}