using INV.Domain.Entities.WareHouses;
using Microsoft.Data.SqlClient;

namespace INV.Infrastructure.Storage.WareHousesStorages
{
    public partial class WareHouseStorage
    { 
        private static WareHouse getWareHouseData(SqlDataReader reader)
        {
            return new WareHouse
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
            };
        }

    }
}