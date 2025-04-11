using INV.App.Products;
using INV.Domain.Entities.Products;
using Microsoft.AspNetCore.Components;
using INVUIs.Products.ProductsModel;
using Radzen;
using Radzen.Blazor;
using INVUIs.Purchases.PurchaseModels;
using INVUIs.Shared;
using INVUIs.Shared.MyAlert;
using Microsoft.JSInterop;
using INV.App.Purchases;

namespace INVUIs.Products
{
    public partial class ProductsList
    {
        [Parameter] public EventCallback<PurchaseProductModel> OnCommand { get; set; }
        [Parameter] public List<ProductInfo> Products { get; set; }
        [Inject] public NavigationManager navigationManager { set; get; }
        [Inject] private IProductService ProductService { get; set; }
        [Inject] private IJSRuntime jsRuntime { set; get; }

        private PurchaseProductModel newProduct = new PurchaseProductModel();
        public ProductForm productForm;
        public ConformationForm conformationForm;
        public ProductDetail productEdit;
        public ProductEditForm productEditForm;
        private RadzenDataGrid<ProductInfo> grid;
        private MyAlert? myAlert;
        private Guid productIdToDelete;
        private string errorMessage;
        private string succesMessage;
        private string searchTerm { get; set; } = "";

        private List<ProductInfo> displayedItems =>
           Products?.Where(i =>
               (i?.Designation?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (i?.Quantity.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (i?.UnitMeasure?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (i?.WareHouse?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
           ).ToList() ?? new List<ProductInfo>();

        protected override async Task OnInitializedAsync()
        {
            myAlert = new MyAlert(jsRuntime);
        }

        private async Task OnProductCreated(ProductInfo product)
        {
            Products.Add(product);
            await grid.Reload();
            StateHasChanged();
        }

        public async Task navigatepage(Guid id) => Navigation.NavigateTo($"/products/{id}");

        private async Task DeleteProduct(Guid productId)
        {
            productIdToDelete = productId;
            var result = await ProductService.RemoveProduct(productIdToDelete);
            if (result.IsSuccess)
            {
                conformationForm.show();
            }
            else
            {
                errorMessage = result.Error.Description;
                await myAlert.ShowAlert(Localizer["ErrorDelete"] , errorMessage, MyAlertType.error);
            }
          
        }

        private async Task ConfirmDeleteProduct()
        {
            var productToRemove = Products.Find(p => p.Id == productIdToDelete);
            var result = await ProductService.RemoveProduct(productIdToDelete);
            if (result.IsSuccess)
            {
                Products.Remove(productToRemove);
                await myAlert.ShowAlert(Localizer["DeleteSuccessfully"], Localizer["Product.Deleted"], MyAlertType.success);
                await grid.Reload();
            }
            else
            {
                errorMessage = result.Error.Description;
                await myAlert.ShowAlert(Localizer["ErrorDelete"], errorMessage, MyAlertType.error);
            }
            StateHasChanged();
        }
    }
}