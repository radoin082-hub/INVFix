using Microsoft.SqlServer.Types;

namespace INV.Domain.Entities.WareHouses;

public class WareHouse
{
    public Guid Id { set; get; }
    public SqlHierarchyId Path { get; set; }
    public string Name { get; set; }
    public WareHouseType WareHouseType { get; set; }
    public string Description { get; set; }
    public SqlHierarchyId? ParentId { get; set; }
    public bool IsExpanded { get; set; } = false;
    public List<WareHouse> Children { get; set; } = new List<WareHouse>();
}