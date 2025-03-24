namespace INV.App.Purchases
{
    public class PurchaseProductInfo
    {
        public Guid PurchaseId { get; set; }
        public Guid ProductId { get; set; }
        public string Designation { get; set; }
        public int TVA { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int Received { get; set; }
        public string WareHouse { get; set; }
    }
}