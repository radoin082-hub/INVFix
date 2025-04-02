using INV.App.Suppliers;
using INVUIs.Suppliers;
using INVUIs.Suppliers.Models;
using Microsoft.AspNetCore.Components;

namespace INVUIs.Purchases;

public partial class PurchaseSupplier
{
    [CascadingParameter] public bool canEdit { get; set; }
    [Parameter] public EventCallback<SupplierInfo> OnSupplierSelected { get; set; }
    [Parameter] public SupplierInfo Supplier { get; set; }
    [Inject] public ISupplierService supplierService { get; set; }

    public SupplierInfo selectedSupplier = null;
    private SupplierForm supplierForm;
    private List<SupplierInfo> supplierInfo = new();
    private SupplierModel supplierModel = new();
    public SupplierSelector supplierSelector;

    private bool isSupplierSelected = false;
    private bool supplierSelect = false;

    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(500);
        selectedSupplier = Supplier;
        var result = await supplierService.GetAllSupplier();
        if (result.IsSuccess)
        {
            supplierInfo = result.Value;
        }
    }

    public async Task supplierSelected(SupplierInfo supplier)
    {
        if (supplier != null)
        {
            selectedSupplier = supplier;
            await OnSupplierSelected.InvokeAsync(supplier);
            StateHasChanged();
        }
    }
}