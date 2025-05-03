using INV.App.WareHouses;
using INV.Domain.Entities.WareHouses;
using INV.Implementation.Hubs.WareHouses;
using INVUIs.Shared;
using INVUIs.WareHouses.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.SqlServer.Types;
using ORG.App.Orgs;
using ORG.Domain.Entities.Node;
using OrgUis.Models;

namespace INV.Web.Components.Pages.Warhouses;

public partial class WarhouseLisPage : IAsyncDisposable
{
    [Inject] private IWareHouseService wareHouseService { set; get; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    
    private WareHouseModel wareHouseModel = new();
    private List<WareHouse> tree = new();
    private HubConnection? hubConnection;
    private bool visible;
    private WareHouse Parentcurrent;
    private string keyframes;
    private Timer? _pollingTimer;
    private readonly int _pollingInterval = 5000; 

    
    protected override async Task OnInitializedAsync()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/warehouseHub"))
            .WithAutomaticReconnect()
            .Build();

        hubConnection.On<WareHouse>("WareHouseAdded", async (warehouse) =>
        {
            await refreshData();
        });

        hubConnection.On<SqlHierarchyId>("WareHouseDeleted", async (id) =>
        {
            await refreshData();
        });
            await hubConnection.StartAsync();
            await loadWareHouses();
        
            _pollingTimer = new Timer(async _ =>
            {
                await refreshData();
            }, null, 0, _pollingInterval);
    }
    private async Task refreshData()
    {
            await loadWareHouses();
            await InvokeAsync(StateHasChanged);
    }
    private async Task<List<WareHouse>> loadWareHouses()
    {
        var result = await wareHouseService.GetAllWareHouses();

        if (result.IsSuccess)
        {
            tree = BuildTree(result.Value ?? new List<WareHouse>());
            return result.Value ?? new List<WareHouse>();
        }

        return new List<WareHouse>();
    }

    private List<WareHouse> BuildTree(List<WareHouse> flat)
    {
        var lookup = flat.ToDictionary(n => n.Id.ToString(), n => n);
        var roots = new List<WareHouse>();
        foreach (var n in flat)
        {
            var parentId = n.Path.GetAncestor(1).ToString();
            if (lookup.TryGetValue(parentId, out var p))
                p.Children.Add(n);
            else
                roots.Add(n);
        }

        return roots;
    }

    private async Task AddRoot() => await ShowPopup(null);
    private async Task AddChild(WareHouse p) => await ShowPopup(p);

    private Task ShowPopup(WareHouse parent)
    {
        Parentcurrent = parent;
        wareHouseModel = new WareHouseModel();
        visible = true;
        return Task.CompletedTask;
    }

    private async Task AddNode()
    {
        var wareHouse = new WareHouse
        {
            Name = wareHouseModel.Name,
            WareHouseType = wareHouseModel.WareHouseType,
            Description = wareHouseModel.Description
        };
        await wareHouseService.CreateWarehouse(wareHouse, Parentcurrent?.Path);
        visible = false;
    }

    private void CancelAddNode() => visible = false;
    private void DropDownNode(WareHouse n) => n.IsExpanded = !n.IsExpanded;

    private async Task NodeDropped((WareHouse draggedNode, WareHouse targetNode) data)
    {
        /*var (draggedNode, targetNode) = data;

        if (draggedNode is not null && targetNode is not null)
        {
           
        }*/
    }

    public async ValueTask DisposeAsync()
    {
        if (_pollingTimer is not null)
        {
            await _pollingTimer.DisposeAsync();
        }
    
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }
    }
}