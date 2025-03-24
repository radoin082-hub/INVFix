using System.ComponentModel.DataAnnotations;
using INVUIs.Products.ProductsModel;

namespace INVUIs.Purchases.PurchaseModels
{
    public class PurchaseModel
    {
        [Required(ErrorMessage = "Please select an article.")]
        public string selectedArticle { get; set; }

        public int ArticleCode { get; set; }
        
        public int ChapterCode { get; set; }

        public string description_article { get; set; }

        public string title_chapter { get; set; }

        [Required(ErrorMessage = "Please select a chapter.")]
        public string selectedChapter { get; set; }


        [Required(ErrorMessage = "Please select a budget category.")]
        public string selectedCategory { get; set; }

        [Required(ErrorMessage = "Please select a service.")]
        public string selectedService { get; set; }

        [Required(ErrorMessage = "Delivery time is required.")]
        [RegularExpression(@"^\d{1,9}$", ErrorMessage = "Invalid delivery time format.")]
        public string DeliveryTime { get; set; }

        public List<PurchaseProductModel> ProductModels { get; set; } = new();

        public decimal TotalHT => ProductModels.Sum(p => p.Quantity * p.UnitPrice);

        public decimal TotalTVA => ProductModels.Sum(p => p.Quantity * p.UnitPrice * p.TVA) / 100;

        public decimal TotalTTC => TotalHT + TotalTVA;
    }
}