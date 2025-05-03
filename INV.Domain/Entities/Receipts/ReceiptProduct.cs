using INV.Domain.Entities.WareHouses;
using Microsoft.SqlServer.Types;

namespace INV.Domain.Entities.Receipts;

public class ReceiptProduct
{
    public Guid ReceptionId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public Guid WareHouseId { get; set; }
  
}