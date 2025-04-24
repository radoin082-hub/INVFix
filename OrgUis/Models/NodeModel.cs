using Microsoft.SqlServer.Types;
using ORG.Domain.Entities.Node;

namespace OrgUis.Models;

public class NodeModel
{
    public SqlHierarchyId Id { get; set; }
    public string Name { get; set; }
    public NodeType NodeType { get; set; }
    public string Description { get; set; }
    public int? ParentId { get; set; }
    public bool IsExpanded { get; set; } = false;
    public List<Node> Children { get; set; } = new List<Node>();
}