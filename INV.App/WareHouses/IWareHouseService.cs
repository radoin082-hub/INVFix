using INV.App.Receipts;
using INV.Domain.Entities.WareHouses;
using INV.Domain.Shared;
using Microsoft.SqlServer.Types;

namespace INV.App.WareHouses
{
    public interface IWareHouseService
    {
        ValueTask<Result<List<WareHouse>>> GetAllWareHouses();

        ValueTask<Result> CreateWarehouse(WareHouse wareHouse, SqlHierarchyId? parentId = null);
        ValueTask<Result<List<WareHouse>>> GetAllWareHousesByWarhouseType();
        ValueTask<Result> UpdateWarhouse(WareHouse wareHouse);
        ValueTask<Result> DeleteWarehouse(SqlHierarchyId id);
    }
}