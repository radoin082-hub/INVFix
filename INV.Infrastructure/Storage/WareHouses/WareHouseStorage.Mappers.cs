using INV.Domain.Entities.WareHouses;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Types;

namespace INV.Infrastructure.Storage.WareHouses;

public partial class WareHouseStorage
{
    private static WareHouse getWareHouseData(SqlDataReader reader)
    {
        return new WareHouse
        {
            Id = (Guid)reader["Id"],
            Path = (SqlHierarchyId)reader["Path"],
            Name = (string)reader["name"],
            Description = (string)reader["Description"],
            WareHouseType = (WareHouseType)reader["type"]
        };
    }
}