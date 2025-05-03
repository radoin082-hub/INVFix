using System.Data;
using INV.Domain.Entities.WareHouses;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Types;

namespace INV.Infrastructure.Storage.WareHouses
{
    public partial class WareHouseStorage(IConfiguration configuration) : IWareHouseStorage
    {
        private readonly string? connectionString = configuration.GetConnectionString("INV");

        public async Task<List<WareHouse>> SelectAllWareHouses()
        {
            var list = new List<WareHouse>();
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(WareHouseStorage.SelectAllWareHousesQuery, conn);
            var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(WareHouseStorage.getWareHouseData(reader));
            }

            return list;
        }

        public async Task<List<WareHouse>> SelectWareHousesByWarehouseType()
        {
            var list = new List<WareHouse>();
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(WareHouseStorage.GetWarhousesByWarehouseType, conn);
            cmd.Parameters.AddWithValue("@aType", WareHouseType.Warehouse);
            var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(WareHouseStorage.getWareHouseData(reader));
            }

            return list;
        }

        public async Task InsertWareHouse(WareHouse node, SqlHierarchyId? parentId = null)
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            SqlHierarchyId newId;

            if (parentId == null)
            {
                var getMax = new SqlCommand(WareHouseStorage.GetMaxRootIdQuery, conn);
                var result = await getMax.ExecuteScalarAsync();
                SqlHierarchyId lastId = result != DBNull.Value ? (SqlHierarchyId)result : SqlHierarchyId.Null;
                newId = SqlHierarchyId.GetRoot().GetDescendant(lastId, SqlHierarchyId.Null);
            }
            else
            {
                var getMax = new SqlCommand(WareHouseStorage.GetMaxChildIdQuery, conn);
                getMax.Parameters.Add(new SqlParameter("@aParent", SqlDbType.Udt)
                {
                    UdtTypeName = "HierarchyId",
                    Value = parentId
                });

                var result = await getMax.ExecuteScalarAsync();
                SqlHierarchyId lastChild = result != DBNull.Value ? (SqlHierarchyId)result : SqlHierarchyId.Null;
                newId = parentId.Value.GetDescendant(lastChild, SqlHierarchyId.Null);
            }

            var insert = new SqlCommand(WareHouseStorage.InsertWareHouseQuery, conn);
            insert.Parameters.AddWithValue("@aId", Guid.NewGuid());
            insert.Parameters.Add(new SqlParameter("@aPath", SqlDbType.Udt)
            {
                UdtTypeName = "HierarchyId",
                Value = newId
            });
            insert.Parameters.AddWithValue("@aName", node.Name);
            insert.Parameters.AddWithValue("@aDesc", node.Description);
            insert.Parameters.AddWithValue("@aType", node.WareHouseType);

            await insert.ExecuteNonQueryAsync();
        }

        public async Task UpdateNodeParent(SqlHierarchyId draggedNodeId, SqlHierarchyId targetNodeId)
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            var getMax = new SqlCommand(WareHouseStorage.GetMaxTargetIdQuery, conn);
            getMax.Parameters.Add(new SqlParameter("@targetId", SqlDbType.Udt)
            {
                UdtTypeName = "HierarchyId",
                Value = targetNodeId
            });

            var lastChild = await getMax.ExecuteScalarAsync();
            var lastChildId = lastChild != DBNull.Value ? (SqlHierarchyId)lastChild : SqlHierarchyId.Null;

            var newPosition = targetNodeId.GetDescendant(lastChildId, SqlHierarchyId.Null);

            var update = new SqlCommand(WareHouseStorage.UpdateNodeParentQuery, conn);
            update.Parameters.Add(new SqlParameter("@acurrentId", SqlDbType.Udt)
            {
                UdtTypeName = "HierarchyId",
                Value = draggedNodeId
            });
            update.Parameters.Add(new SqlParameter("@anewId", SqlDbType.Udt)
            {
                UdtTypeName = "HierarchyId",
                Value = newPosition
            });

            await update.ExecuteNonQueryAsync();
        }
        public async Task UpdateWareHouse(WareHouse node)
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
        
            var cmd = new SqlCommand(WareHouseStorage.UpdateWareHouseQuery, conn);
           
            cmd.Parameters.Add(new SqlParameter("@aPath", SqlDbType.Udt)
            {
                UdtTypeName = "HierarchyId",
                Value = node.Id
            });
            cmd.Parameters.AddWithValue("@aName", node.Name);
            cmd.Parameters.AddWithValue("@aDesc", node.Description);
            cmd.Parameters.AddWithValue("@aType", node.WareHouseType);
        
            await cmd.ExecuteNonQueryAsync();
        }
        
        public async Task DeleteWareHouse(SqlHierarchyId id)
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
        
            var cmd = new SqlCommand(WareHouseStorage.DeleteWareHouseQuery, conn);
            cmd.Parameters.Add(new SqlParameter("@aPath", SqlDbType.Udt)
            {
                UdtTypeName = "HierarchyId",
                Value = id
            });
        
            await cmd.ExecuteNonQueryAsync();
        }
        public async Task<bool> HasChildren(SqlHierarchyId id)
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(CheckChildrenQuery, conn);
            cmd.Parameters.Add(new SqlParameter("@aPath", SqlDbType.Udt)
            {
                UdtTypeName = "HierarchyId",
                Value = id
            });

            var count = (int)(await cmd.ExecuteScalarAsync())!;
            return count > 0;
        }
    }
}