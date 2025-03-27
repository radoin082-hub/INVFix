using INV.App.Suppliers;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace INVUIs.Suppliers;

public partial class SupplierSelector
{
    [Parameter] public string Title { set; get; }
    [Parameter] public EventCallback<SupplierInfo> OnSelected { set; get; }
    [Parameter] public List<SupplierInfo> Supplier { set; get; }
    [Parameter] public Guid SupplierId { get; set; }
    [Inject] public ISupplierService supplierService { set; get; }

    private IEnumerable<SupplierInfo> displayedItems = new List<SupplierInfo>();
    private RadzenDataGrid<SupplierInfo> grid;
    private SupplierInfo selectedSupplier = new();
    private SupplierForm supplierForm;

    protected override async Task OnInitializedAsync()
    {
        await loadSuppliers();
    }

    protected override async Task OnParametersSetAsync()
    {
    }

    private async Task loadSuppliers()
    {
        var result = await supplierService.GetAllSupplier();
        if (result.IsSuccess)
        {
            displayedItems = result.Value.ToList();
        }
        StateHasChanged();
    }

    private async Task selectRowSepplier(SupplierInfo supplier)
    {
        if (supplier != null)
        {
            selectedSupplier = supplier;
            await OnSelected.InvokeAsync(supplier);
        }
    }

    private async Task OnSupplierSelected(SupplierInfo newSupplier)
    {
        await loadSuppliers();
        selectedSupplier = newSupplier;
        await OnSelected.InvokeAsync(newSupplier);
        StateHasChanged();
    }
}