using INV.Domain.Entities.Budget;
using INV.Domain.Entities.Products;
using INV.Domain.Entities.Suppliers;

namespace INV.App.Purchases;

public class PurchaseDetail
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
    public Supplier Supplier { set; get; }
    public List<Product> Products = new List<Product>();
}