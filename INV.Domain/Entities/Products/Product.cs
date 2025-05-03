using Microsoft.SqlServer.Types;

namespace INV.Domain.Entities.Products
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid DefaultWareHouseId { get; set; }
        public string Designation { get; set; }
        public string UnitMeasure { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int TVA { get; set; }
        public string WareHouse { get; set; }
      
    }
}