using INV.App.Purchases;
using INV.Shared;
using Microsoft.AspNetCore.Components;
using System.Linq;
using Microsoft.JSInterop;
using Radzen.Blazor;

namespace INVUIs.Purchases
{
    public partial class PurchaseList
    {
        [Parameter] public List<PurchaseOrderInfo> purchaseOrderInfos { get; set; }
        [Parameter] public RenderFragment Pills { get; set; }
        [Parameter] public bool ShowSupplier { get; set; } = true;
        [Inject] public IJSRuntime jsRuntime { set; get; }
        [Inject] public HttpClient httpClient { set; get; }
        /*public RadzenDataGrid<PurchaseOrderInfo> grid=new RadzenDataGrid<PurchaseOrderInfo>();

        protected override async Task OnInitializedAsync()
        {
             grid.Responsive = true;
        }*/

        public async Task navigatepage(Guid id) => Navigation.NavigateTo($"{PageRoutes.Purchases}/{id}");

        private string SearchTerm { get; set; } = "";

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

        private async Task downloadFile(Guid Id)
        {
            var responce = $"http://localhost:5095/purchase/DownloadPdf/{Id}/Ar";
            var res = await httpClient.GetAsync(responce);
            if (res.IsSuccessStatusCode)
            {
                var bytes = await res.Content.ReadAsByteArrayAsync();
                await jsRuntime.InvokeVoidAsync("openPdf", bytes);
            }
        }
    }
}