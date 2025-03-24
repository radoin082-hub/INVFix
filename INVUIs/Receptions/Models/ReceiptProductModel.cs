namespace INVUIs.Receptions.Models;

internal class ReceiptProductModel
{
    public Guid ProductId { get; set; }
    public int Received { get; set; }
    
    public int NEwReceived { get; set; }
    public int Quantity { get; set; }
    public string Designation { get; set; }
    public decimal UnitPrice { set; get; }
}