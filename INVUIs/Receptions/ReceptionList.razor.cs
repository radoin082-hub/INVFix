using INV.App.Receipts;
using INV.Domain.Entities.Receipts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using INVUIs.Receptions.Models;
using INV.Domain.Shared;
using INV.Shared;
using System.Linq;
using Microsoft.JSInterop;

namespace INVUIs.Receptions
{
    public partial class ReceptionList
    {
        [Parameter] public EventCallback<ReceptionModel> OnCommand { get; set; }
        [Parameter] public List<ReceiptInfo> Receptions { get; set; }
        [Parameter] public RenderFragment Pills { get; set; }
        [Parameter] public bool ShowPurchase { get; set; } = true;
        [Parameter] public bool ShowSupplier { get; set; } = true;
        [Inject] public NavigationManager navigationManager { set; get; }

        public async Task navigatepage(Guid id) => Navigation.NavigateTo($"receptions/{id}");

        private ReceptionModel commandshow = new ReceptionModel();

        public EditContext editContext { get; set; }
        private bool CommandSelected = false;

        private string searchTerm;

        public string SearchTerm
        {
            get => searchTerm;
            set
            {
                if (searchTerm != value)
                {
                    searchTerm = value;
                    FilterReceptions();
                }
            }
        }

        private List<ReceiptInfo> filteredReceptions;

        public List<ReceiptInfo> FilteredReceptions
        {
            get => filteredReceptions ?? Receptions;
            set => filteredReceptions = value;
        }

        private void FilterReceptions()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                FilteredReceptions = Receptions;
            }
            else
            {
                FilteredReceptions = Receptions
                    .Where(r => (r.Number?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                (r.supplierName?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                (r.purchaseNumber?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                (r.DeliveryNumber?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                r.Status.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                (r.Date.HasValue && r.Date.Value.ToString()
                                    .Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                                (r.DeliveryDate.HasValue && r.DeliveryDate.Value.ToString()
                                    .Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }
        }

        [Inject] private HttpClient HttpClient { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private IJSRuntime jsRuntime { get; set; }

        private async Task DownloadFile(Guid id)
        {
            var url = $"http://localhost:5095/receipt/DownloadPdf/{id}/Fr";
            /*
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("API-KEY", "Bh9OAJZp1Wu8LeNlbZXDfGQGvLc5iY5jd9sXwf15mFbPUnPNenM7yDfYR0JYGPCBwiVdHFMXux47UubKzLkxl8qKJmSasD39qAjqJh6KIpgS6PFXE6b431AuD186OQwB");
            */
            var response = await HttpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsByteArrayAsync();
                await jsRuntime.InvokeVoidAsync("openPdf", content);
            }
        }
    }
}