using Microsoft.SqlServer.Types;

namespace ORG.Domain.Entities.Node;

public class Node
{
    public SqlHierarchyId Id { get; set; }
    public string Name { get; set; }
    public NodeType Type { get; set; }
    public string Description { get; set; }
    public int? ParentId { get; set; }
    public bool IsExpanded { get; set; } = false;
    public List<Node> Children { get; set; } = new List<Node>();
}