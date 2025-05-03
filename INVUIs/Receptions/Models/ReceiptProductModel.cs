using INV.App.Receipts;
using Microsoft.SqlServer.Types;

namespace INVUIs.Receptions.Models;

public class ReceiptProductModel
{
    public Guid ProductId { get; set; }
    public Guid ReceiptId { get; set; }
    public int Received { get; set; }
    
    public int NEwReceived { get; set; }
    public int Quantity { get; set; }
    public string Designation { get; set; }
    public decimal UnitPrice { set; get; }
    public Guid WareHouseId { set; get; }
    public List<ReceiptProductDetails> ReceiptProductDetails { get; set; } = new();

}