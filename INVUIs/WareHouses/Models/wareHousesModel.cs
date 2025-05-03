using System.ComponentModel.DataAnnotations;
using INV.Domain.Entities.WareHouses;
using Microsoft.SqlServer.Types;
namespace INVUIs.WareHouses.Models;

public class WareHouseModel
{


    public Guid Id { get; set; }
    public SqlHierarchyId Path { set; get; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Code is required")]
    public WareHouseType WareHouseType { get; set; }
    [Required(ErrorMessage = "Code is required")]
    public string Description { get; set; }
    public int? ParentId { get; set; }
    public bool IsExpanded { get; set; } = false;
    public List<WareHouse> Children { get; set; } = new List<WareHouse>();
  
}