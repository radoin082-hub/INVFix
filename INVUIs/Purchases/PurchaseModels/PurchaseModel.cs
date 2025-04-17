using System.ComponentModel.DataAnnotations;
using INV.Domain.Entities.Budget;
using INV.Domain.Entities.Purchases;
using INVUIs.Products.ProductsModel;

namespace INVUIs.Purchases.PurchaseModels
{
    public class PurchaseModel
    {
        public Guid SupplierId { get; set; }

        [Required(ErrorMessage = "Please select an DateC.")]
        public DateOnly Date { set; get; } = DateOnly.FromDateTime(DateTime.Now);

        public Guid Id { get; set; }

        public string selectedArticle { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a Article Code.")]
        public int ArticleCode { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a Chapter.")]
        public int ChapterCode { get; set; }

        public string description_article { get; set; }

        public string title_chapter { get; set; }

        [Required(ErrorMessage = "Please select a budget category.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a Category.")]
        public BudgeType selectedCategory { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a Service.")]
        public ServiceType selectedService { get; set; }

        [Required(ErrorMessage = "Delivery time is required.")]
        [RegularExpression(@"^\d{1,9}$", ErrorMessage = "Invalid delivery time format.")]
        public string DeliveryTime { get; set; }

        public DateOnly? VisaDate { get; set; }
        public string VisaNumber { get; set; }
        public string Observation { get; set; }
        public PurchaseStatus Status { set; get; }

        public List<PurchaseProductModel> ProductModels
        { get; set; } = new();

        public decimal TotalHT => ProductModels.Sum(p => p.Quantity * p.UnitPrice);

        public decimal TotalTVA => ProductModels.Sum(p => p.Quantity * p.UnitPrice * p.TVA) / 100;

        public decimal TotalTTC => TotalHT + TotalTVA;
    }
}