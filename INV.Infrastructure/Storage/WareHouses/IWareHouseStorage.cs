using INV.Domain.Entities.WareHouses;
using Microsoft.SqlServer.Types;

namespace INV.Infrastructure.Storage.WareHouses;

public interface IWareHouseStorage
{
    Task<List<WareHouse>> SelectAllWareHouses();
    Task<List<WareHouse>> SelectWareHousesByWarehouseType();
    Task InsertWareHouse(WareHouse node, SqlHierarchyId? parentId = null);
    Task UpdateWareHouse(WareHouse node);
    Task DeleteWareHouse(SqlHierarchyId id);
    Task<bool> HasChildren(SqlHierarchyId id);
}