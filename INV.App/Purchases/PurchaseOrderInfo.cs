using INV.App.Suppliers;
using INV.Domain.Entities.Budget;
using INV.Domain.Entities.Purchases;

namespace INV.App.Purchases
{
    public class PurchaseOrderInfo
    {
        public Guid Id { set; get; }
        public string Number { set; get; }
        public DateOnly Date { set; get; }
        public string BudgeArticle { set; get; }
        public BudgeType BudgeType { set; get; }
        public ServiceType ServiceType { set; get; }
        public Guid SupplierId { set; get; }
        public string SupplierName { set; get; }

        public decimal TotalTTC { get; set; }

        public int CompletionDelay { get; set; }

        public DateOnly? VisaDate { get; set; }
        public string VisaNumber { get; set; }
        public string Observation { get; set; }
        
        public SupplierInfo Supplier { get; set; }
        public PurchaseStatus Status { get; set; }
    }
}