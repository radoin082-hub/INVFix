using System.Diagnostics;
using INV.App.Receipts;
using INV.App.WareHouses;
using INV.Domain.Entities.WareHouses;
using INVUIs.Receptions.Models;
using INVUIs.Shared.MyAlert;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.SqlServer.Types;

namespace INVUIs.Receptions;

public partial class ReceptionStorageLocation
{
    [Parameter] public ReceiptProductModel? ReceiptProductModels { get; set; }
    [Parameter] public List<WareHouse>? WareHouses { set; get; }
    [Parameter] public EventCallback<List<ReceiptProductDetails>> OnLocationsSaved { get; set; }
    [Inject] private IWareHouseService wareHouseService { set; get; }
    [Inject] private IJSRuntime jsRuntime { set; get; }
    private List<ReceiptProductDetails> storageLocations = new List<ReceiptProductDetails>();
    private Dictionary<ReceiptProductDetails, SqlHierarchyId> previousWarehouseSelections = new();
    private MyAlert? myAlert;
    private bool Visible { get; set; } = false;

    protected override Task OnInitializedAsync()
    {
        myAlert = new MyAlert(jsRuntime);
        return Task.CompletedTask;
    }

    public void Hide()
    {
        Visible = false;
        StateHasChanged();
    }

    public Task Show(ReceiptProductModel? model = null!)
    {
        ReceiptProductModels = model ?? ReceiptProductModels;

        storageLocations.Clear();

        string warehouseName;

        var warehouse = WareHouses.FirstOrDefault(x =>
            x.Id.ToString() == ReceiptProductModels.WareHouseId.ToString());
        warehouseName = warehouse?.Name ?? string.Empty;


        storageLocations.Add(new ReceiptProductDetails()
        {
            WarhouseId = ReceiptProductModels.WareHouseId,
            WarhouseName = warehouseName,
            Quantity = ReceiptProductModels.Received
        });

        Visible = true;

        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task SaveLocations()
    {
        int total = storageLocations.Sum(x => x.Quantity);
        if (storageLocations.Any(x => x.Quantity == 0))
        {
            await myAlert.ShowToast("Error", "Item quantity cannot be zero.", MyAlertType.error);
            return;
        }

        if (total > ReceiptProductModels.Quantity || total == 0)
        {
            await myAlert.ShowToast("Error", "The total quantity exceeds the received quantity Or Zero.",
                MyAlertType.error);
            return;
        }

        if (storageLocations.Any(x => x.WarhouseId.Equals(Guid.Empty)))
        {
            await myAlert.ShowToast("Error", "Warehouse must be selected for all items.", MyAlertType.error);
            return;
        }

        ReceiptProductModels.ReceiptProductDetails = storageLocations
            .Select(x => new ReceiptProductDetails
            {
                ReceiptProductId = ReceiptProductModels.ReceiptId,
                ProductId = ReceiptProductModels.ProductId,
                WarhouseId = x.WarhouseId,
                WarhouseName = x.WarhouseName,
                Quantity = x.Quantity
            }).ToList();
        if (OnLocationsSaved.HasDelegate)
            await OnLocationsSaved.InvokeAsync(ReceiptProductModels.ReceiptProductDetails);

        Hide();
    }

    private IEnumerable<WareHouse> GetAvailableWarehouses(ReceiptProductDetails currentItem)
    {
        var selectedIds = storageLocations
            .Where(x => !x.WarhouseId.Equals(Guid.Empty) && x != currentItem)
            .Select(x => x.WarhouseId)
            .ToHashSet();

        return WareHouses.Where(w => !selectedIds.Contains(w.Id));
    }

    private void OnWarehouseChanged(ReceiptProductDetails item, object value) => StateHasChanged();

    private void AddNewItem()
    {
        storageLocations.Add(new ReceiptProductDetails());
        StateHasChanged();
    }

    private void DeleteItem(ReceiptProductDetails item)
    {
        storageLocations.Remove(item);
        StateHasChanged();
    }
}