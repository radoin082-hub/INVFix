using INV.App.Receipts;
using INV.App.WareHouses;
using INV.Domain.Entities.WareHouses;
using INV.Domain.Shared;
using INV.Implementation.Hubs.WareHouses;
using INV.Infrastructure.Storage.WareHouses;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Types;

namespace INV.Implementation.Service.WareHouses;

public class WareHouseService(IWareHouseStorage wareHouseStorage,IServiceProvider serviceProvider) : IWareHouseService
{
    public async ValueTask<Result<List<WareHouse>>> GetAllWareHouses()
    {
        try
        {
            return await wareHouseStorage.SelectAllWareHouses();
        }
        catch (Exception e)
        {
            return Error.Exception(e);
        }
    }

    public async ValueTask<Result<List<WareHouse>>> GetAllWareHousesByWarhouseType()
    {
        try
        {
            return await wareHouseStorage.SelectWareHousesByWarehouseType();

        }
        catch (Exception e)
        {
            return Error.Exception(e);
        }
    }

    public async ValueTask<Result> CreateWarehouse(WareHouse wareHouse, SqlHierarchyId? parentId = null)
    {
        try
        {
            await wareHouseStorage.InsertWareHouse(wareHouse, parentId);
            var hub = serviceProvider.GetRequiredService<IHubContext<WareHouseHub>>();
            await hub.Clients.All.SendAsync("WareHouseAdded", wareHouse);

            return Result.Success();
        }
        catch (Exception e)
        {
            return Error.Exception(e);
        }
    }

    public async ValueTask<Result> UpdateWarhouse(WareHouse wareHouse)
    {
        try
        {
            await wareHouseStorage.UpdateWareHouse(wareHouse);
            return Result.Success();
        }
        catch (Exception e)
        {
            return Error.Exception(e);
        }
    }

 

    public async ValueTask<Result> DeleteWarehouse(SqlHierarchyId id)
    {
        try
        {
            var childerCount = await wareHouseStorage.HasChildren(id);
            if (childerCount)
            {
                return Error.Failure("Delete Error","This warehouse has children, please delete them first.");
            }
            await wareHouseStorage.DeleteWareHouse(id);
            var hub = serviceProvider.GetRequiredService<IHubContext<WareHouseHub>>();
            await hub.Clients.All.SendAsync("WareHouseAdded", id);

            return Result.Success();
        }
        catch (Exception e)
        {
            return Error.Exception(e);
        }
    }
}