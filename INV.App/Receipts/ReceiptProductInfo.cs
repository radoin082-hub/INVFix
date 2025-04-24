namespace INV.App.Receipts;

public class ReceiptProductInfo
{
    public Guid ReceptionId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public int Received { get; set; }
    public string Designation { get; set; }
    public string UnitMeasure { get; set; }
    public decimal UnitPrice { set; get; }
    public Guid DefaultWareHouseId { get; set; }
    public string PurchaseNumber { get; set; }
    
}