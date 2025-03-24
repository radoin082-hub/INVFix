using INV.App.Purchases;
using INV.Domain.Entities.Purchases;
using Microsoft.AspNetCore.Components;

namespace INVUIs.Purchases;

public partial class PurchaseCard
{
    [Parameter] public PurchaseOrderInfo purchaseInfo { set; get; }
    [Inject] public IPurchaseOrderService purchaseOrderService { get; set; }
    private bool displayVisa = false;
    private bool displayReject = false;

    private void visa(PurchaseOrderInfo purchaseInfo)
    {
        displayVisa = false;
        purchaseInfo.Status = PurchaseStatus.Vised;

        purchaseOrderService.DecisionCF(purchaseInfo.Id, purchaseInfo.Status, purchaseInfo.VisaDate, purchaseInfo.VisaNumber, null);
        StateHasChanged();
    }

    private void reject(PurchaseOrderInfo purchaseInfo)
    {
        displayReject = false;
        purchaseInfo.Status = PurchaseStatus.Reject;
        purchaseOrderService.DecisionCF(purchaseInfo.Id, purchaseInfo.Status, purchaseInfo.VisaDate, purchaseInfo.VisaNumber, purchaseInfo.Observation);
        StateHasChanged();
    }

    public void ShowVisa()
    {
        displayVisa = true;
        StateHasChanged();
    }

    public void HideVisa()
    {
        purchaseInfo.VisaDate = null;
        purchaseInfo.VisaNumber = null;
        displayVisa = false;
        StateHasChanged();
    }

    public void Showreject()
    {
        purchaseInfo.VisaDate = null;
        purchaseInfo.VisaNumber = null;
        purchaseInfo.Observation = null;
        displayReject = true;
        StateHasChanged();
    }

    public void HideReject()
    {
        purchaseInfo.VisaDate = null;
        purchaseInfo.VisaNumber = null;
        purchaseInfo.Observation = null;
        displayReject = false;
        StateHasChanged();
    }
}