using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Types;
using ORG.Domain.Entities.Node;

namespace ORG.Infrastructure.Storage.Orgs;

public class OrgStorage(IConfiguration configuration) : IOrgStorage

{
    private readonly string connectionString = configuration.GetConnectionString("INV");

    public async Task<List<Node>> SelectAllOrgs()
    {
        var list = new List<Node>();
        using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        var cmd = new SqlCommand("SELECT Id, Name, Description, Type FROM ORG", conn);
        var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Node
            {
                Id = (SqlHierarchyId)reader["Id"],
                Name = (string)reader["name"],
                Description = (string)reader["Description"],
                Type = (NodeType)reader["type"]
            });
        }

        return list;
    }

    public async Task InsertOrg(Node node, SqlHierarchyId? parentId = null)
    {
        using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        SqlHierarchyId newId;

        if (parentId == null)
        {
            var getMax = new SqlCommand("select max(Id) from ORG where Id.GetAncestor(1) = hierarchyid::GetRoot()",
                conn);
            var result = await getMax.ExecuteScalarAsync();
            SqlHierarchyId lastId = result != DBNull.Value ? (SqlHierarchyId)result : SqlHierarchyId.Null;
            newId = SqlHierarchyId.GetRoot().GetDescendant(lastId, SqlHierarchyId.Null);
        }
        else
        {
            var getMax = new SqlCommand("select max(Id) from ORG where Id.GetAncestor(1) = @aParent", conn);
            getMax.Parameters.Add(new SqlParameter("@aParent", SqlDbType.Udt)
            {
                UdtTypeName = "HierarchyId",
                Value = parentId
            });

            var result = await getMax.ExecuteScalarAsync();
            SqlHierarchyId lastChild = result != DBNull.Value ? (SqlHierarchyId)result : SqlHierarchyId.Null;
            newId = parentId.Value.GetDescendant(lastChild, SqlHierarchyId.Null);
        }

        var insert = new SqlCommand(
            "INSERT INTO ORG (Id, Name, Description, Type) VALUES (@aId, @aName, @aDesc, @aType)",
            conn);
        insert.Parameters.Add(new SqlParameter("@aId", SqlDbType.Udt)
        {
            UdtTypeName = "HierarchyId",
            Value = newId
        });
        insert.Parameters.AddWithValue("@aName", node.Name);
        insert.Parameters.AddWithValue("@aDesc", node.Description);
        insert.Parameters.AddWithValue("@aType", node.Type);

        await insert.ExecuteNonQueryAsync();
    }

    public async Task UpdateNodeParent(SqlHierarchyId draggedNodeId, SqlHierarchyId targetNodeId)
    {
        using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();


        var getMax = new SqlCommand("SELECT MAX(Id) FROM ORG WHERE Id.GetAncestor(1) = @targetId", conn);
        getMax.Parameters.Add(new SqlParameter("@targetId", SqlDbType.Udt)
        {
            UdtTypeName = "HierarchyId",
            Value = targetNodeId
        });

        var lastChild = await getMax.ExecuteScalarAsync();
        var lastChildId = lastChild != DBNull.Value ? (SqlHierarchyId)lastChild : SqlHierarchyId.Null;


        var newPosition = targetNodeId.GetDescendant(lastChildId, SqlHierarchyId.Null);


        var update = new SqlCommand("UPDATE ORG SET Id = @anewId WHERE Id = @acurrentId", conn);
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
}