using System.ComponentModel.DataAnnotations;
using INV.Domain.Entities.Budget;
using INV.Domain.Entities.Purchases;
using INVUIs.Products.ProductsModel;

namespace INVUIs.Purchases.PurchaseModels
{
    public class PurchaseModel
    {
        public Guid SupplierId { get; set; }
        public DateOnly Date { set; get; }
        public Guid Id { get; set; }

        // [Required(ErrorMessage = "Please select an article.")]
        public string selectedArticle { get; set; }

        public int ArticleCode { get; set; }

        public int ChapterCode { get; set; }

        public string description_article { get; set; }

        public string title_chapter { get; set; }

        ///[Required(ErrorMessage = "Please select a chapter.")]
        //public string selectedChapter { get; set; }

        [Required(ErrorMessage = "Please select a budget category.")]
        public BudgeType selectedCategory { get; set; }

        [Required(ErrorMessage = "Please select a service.")]
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