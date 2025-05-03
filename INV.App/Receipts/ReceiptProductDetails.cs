using Microsoft.SqlServer.Types;

namespace INV.App.Receipts;

public class ReceiptProductDetails
{
    public Guid ReceiptProductId { get; set; }
    public Guid ProductId { get; set; }
    public Guid WarhouseId { get; set; }  
    public string WarhouseName { get; set; }
    public int Quantity { get; set; }
}