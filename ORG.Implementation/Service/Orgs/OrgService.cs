using Microsoft.SqlServer.Types;
using ORG.App.Orgs;
using ORG.Domain.Entities.Node;
using ORG.Domain.Shared;
using ORG.Infrastructure.Storage.Orgs;

namespace ORG.Implementation.Service.Orgs;

public class OrgService(IOrgStorage orgStorage):IOrgService
{
    public async ValueTask <Result<List<Node>>>GetAllOrgs()
    {
        try
        {
            var result=await orgStorage.SelectAllOrgs();
            return Result.Success(result);
        }
        catch (Exception e)
        {
            return Error.Exception(e);
        }
    }
    public async ValueTask<Result> UpdateNodeParent(SqlHierarchyId draggedNodeId, SqlHierarchyId targetNodeId)
    {
        try
        {
            await orgStorage.UpdateNodeParent(draggedNodeId, targetNodeId);
            return Result.Success();
        }
        catch (Exception e)
        {
            return Error.Exception(e);
        }
    }
    public async ValueTask<Result> AddOrg(Node node, SqlHierarchyId? parentId = null)
    {
        try
        {
            await orgStorage.InsertOrg(node,parentId);
            return Result.Success();
        }
        catch (Exception e)
        {
            return Error.Exception(e);
        }
    }
}