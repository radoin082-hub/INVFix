using INV.Domain.Entities.Budget;

namespace INV.Domain.Entities.Purchases
{
    public class PurchaseOrder
    {
        public Guid Id { set; get; }
        public string Number { set; get; }
        public Guid SupplierId { set; get; }
        public DateOnly Date { set; get; }
        public int BudgetChapter { set; get; }
        public int BudgetArticle { set; get; }
        public BudgeType BudgetType { set; get; }
        public ServiceType ServiceType { set; get; }
        public decimal TotalHT { get; set; }
        public decimal TotalTVA { get; set; }
        public decimal TotalTTC { get; set; }
        public int CompletionDelay { set; get; }
        public string? VisaNumber { set; get; }
        public DateOnly? VisaDate { set; get; }

        public string Observation { get; set; }
        public PurchaseStatus Status { set; get; } = PurchaseStatus.Editing;

        public List<PurchaseProduct> Products { get; set; } = new();
    }
}