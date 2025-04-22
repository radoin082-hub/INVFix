using INV.App.Purchases;
using INV.Shared;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace INVUIs.Purchases
{
    public partial class PurchaseList
    {
        [Parameter] public List<PurchaseOrderInfo> purchaseOrderInfos { get; set; }
        [Parameter] public RenderFragment Pills { get; set; }
        [Parameter] public bool ShowSupplier { get; set; } = true;

        public async Task navigatepage(Guid id) => Navigation.NavigateTo($"{PageRoutes.Purchases}/{id}");

        private string SearchTerm { get; set; } = "";

        private void downloadFile(Guid id)
        {
        }

        private List<PurchaseOrderInfo> displayedItems =>
         purchaseOrderInfos?.Where(i =>
             (i?.Number?.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
             (i?.SupplierName?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
             (i?.BudgeArticle?.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
             (i?.BudgeType.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
             (i?.ServiceType.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
             (i?.TotalTTC.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
             (i?.Status.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
             (i?.Date.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
             (i?.Observation?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
             (i?.VisaNumber?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
         ).ToList() ?? new List<PurchaseOrderInfo>();
    }
}