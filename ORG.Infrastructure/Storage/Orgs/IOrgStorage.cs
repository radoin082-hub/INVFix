using Microsoft.SqlServer.Types;
using ORG.Domain.Entities.Node;

namespace ORG.Infrastructure.Storage.Orgs;

public interface IOrgStorage
{
    Task<List<Node>> SelectAllOrgs();
    Task InsertOrg(Node node, SqlHierarchyId? parentId = null);
    Task UpdateNodeParent(SqlHierarchyId draggedNodeId, SqlHierarchyId targetNodeId);
}