namespace INV.Domain.Entities.Receipts;

public class Receipt
{
    public Guid Id { get; set; }
    public Guid PurchaseId { get; set; }
    public DateOnly Date { get; set; }
    public string DeliveryNumber { get; set; }
    public string Number { get; set; }
    public DateOnly DeliveryDate { get; set; }
    public ReceiptStatus Status { get; set; }
    public List<ReceiptProduct> Products { get; set; } = new();
}