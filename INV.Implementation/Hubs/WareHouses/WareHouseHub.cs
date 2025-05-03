using INV.Domain.Entities.WareHouses;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SqlServer.Types;

namespace INV.Implementation.Hubs.WareHouses;

public class WareHouseHub : Hub
{
    public async Task NotifyAdd(WareHouse wareHouse)
    {
        await Clients.All.SendAsync("WareHouseAdded", wareHouse);
    }

    public async Task NotifyDelete(SqlHierarchyId id)
    {
        await Clients.All.SendAsync("WareHouseDeleted", id);
    }
}