using INV.App.Products;
using INV.Domain.Entities.Products;
using Microsoft.AspNetCore.Components;
using INVUIs.Products.ProductsModel;
using Radzen;
using Radzen.Blazor;
using INVUIs.Purchases.PurchaseModels;
using INVUIs.Shared;

namespace INVUIs.Products
{
    public partial class ProductsList
    {
        [Parameter] public EventCallback<PurchaseProductModel> OnCommand { get; set; }
        [Inject] public NavigationManager navigationManager { set; get; }
        [Inject] private IProductService ProductService { get; set; }
        [Parameter] public List<ProductInfo> Products { get; set; }
        public ProductForm productForm;
        public ConformationForm conformationForm;
        public ProductDetail productEdit;
        public ProductEditForm productEditForm;
        private RadzenDataGrid<ProductInfo> grid;
        private Guid productIdToDelete;

        public async Task navigatepage(Guid id) => Navigation.NavigateTo($"/products/{id}");

        private PurchaseProductModel newProduct = new PurchaseProductModel();

        private async Task DeleteProduct(Guid productId)
        {
            productIdToDelete = productId;
            conformationForm.show();
        }

        private async Task ConfirmDeleteProduct()
        {
            var productToRemove = Products.Find(p => p.Id == productIdToDelete);

            if (productToRemove != null)
            {
                Products.Remove(productToRemove);
                var result = await ProductService.RemoveProduct(productIdToDelete);
                await grid.Reload();
                StateHasChanged();
            }
        }
    }
}