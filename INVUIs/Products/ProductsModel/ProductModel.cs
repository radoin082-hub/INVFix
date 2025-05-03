using System.ComponentModel.DataAnnotations;
using Microsoft.SqlServer.Types;

namespace INVUIs.Products.ProductsModel
{
    public class ProductModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public Guid WareHouseId { set; get; }

        [Required(ErrorMessage = "Name Product is required")]
        public string Designation { get; set; }

        [Required(ErrorMessage = "UnitMeasure is required")]
        public string UnitMeasure { get; set; }

        [Required(ErrorMessage = "TVA is required")]
        public int TVA { get; set; }
    }
}