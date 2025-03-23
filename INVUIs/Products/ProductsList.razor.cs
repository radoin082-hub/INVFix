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

        protected override async Task OnInitializedAsync()
        {
            myAlert = new MyAlert(jsRuntime);
        }

        public async Task navigatepage(Guid id) => Navigation.NavigateTo($"/products/{id}");


        private async Task DeleteProduct(Guid productId)
        {
            productIdToDelete = productId;
            conformationForm.show();
        }

        private async Task ConfirmDeleteProduct()
        {
            var productToRemove = Products.Find(p => p.Id == productIdToDelete);
            var result = await ProductService.RemoveProduct(productIdToDelete);
            if (result.IsSuccess)
            {
                Products.Remove(productToRemove);
                await myAlert.ShowErrorAlert("Delete Succusfuly", "The product has been deleted.",MyAlertType.success);
                await grid.Reload();
            }
            else
            {
                errorMessage = result.Error.Description;
                await myAlert.ShowErrorAlert("Error Delete", errorMessage,MyAlertType.error);
            }
            StateHasChanged();
        }
    }
}