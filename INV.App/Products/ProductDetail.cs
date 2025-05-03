using INV.App.Receipts;
using Microsoft.SqlServer.Types;

namespace INV.App.Products
{
    public class ProductDetail
    {
        public Guid Id { get; set; }
        public string Designation { get; set; }
        public string UnitMeasure { get; set; }
        public int Quantity { get; set; }
        public int TVA { get; set; }
        public Guid DefaultWareHouseId { get; set; }
        public string WareHouse { get; set; }
        public List<ReceiptInfo> ReceiptInfos { get; set; } = new();
    }
}