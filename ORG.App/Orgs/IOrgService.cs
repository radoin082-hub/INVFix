using Microsoft.SqlServer.Types;
using ORG.Domain.Entities.Node;
using ORG.Domain.Shared;

namespace ORG.App.Orgs;

public interface IOrgService
{
    ValueTask<Result<List<Node>>> GetAllOrgs();
    ValueTask<Result> AddOrg(Node node, SqlHierarchyId? parentId = null);
    ValueTask<Result> UpdateNodeParent(SqlHierarchyId draggedNodeId, SqlHierarchyId targetNodeId);
}