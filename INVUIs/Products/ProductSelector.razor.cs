using INV.App.Products;
using INV.Domain.Entities.Products;
using INVUIs.Products.ProductsModel;
using INVUIs.Purchases.PurchaseModels;
using Microsoft.AspNetCore.Components;

namespace INVUIs.Products;

public partial class ProductSelector : ComponentBase
{
    private List<ProductInfo> filteredProducts;
    private string filterText;
    private bool isProductSelected = false;
    private ProductForm productForm = new();

    private List<ProductInfo> products;
    private PurchaseProductModel selectedProductModel;

    private bool visibility = false;
    [Parameter] public EventCallback<PurchaseProductModel> OnProductSelected { get; set; }
    [Inject] public IProductService productService { set; get; }
    private Guid? SelectedProductId { get; set; } = null;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public void ShowModal()
    {
        visibility = true;
        StateHasChanged();
    }

    public void ShowModal(ProductInfo product)
    {
        selectedProductModel = new PurchaseProductModel
        {
            Id = product.Id,
            Designation = product.Designation,
            UnitMeasure = product.UnitMeasure,
            TVA = product.TVA,
            /*     UnitPrice = product.UnitPrice,*/
            Quantity = product.Quantity
        };
        visibility = true;
        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        LoadProducts();
    }

    public async Task LoadProducts()
    {
        var result = await productService.GetProducts();
        if (result.IsSuccess)
        {
            products = result.Value;
        }
        filterProducts();
    }

    private void filterProducts()
    {
        if (string.IsNullOrEmpty(filterText))
            filteredProducts = products.ToList();
        else
            filteredProducts = products
                .Where(p => p.Designation?.ToLower().Contains(filterText.ToLower()) ?? false)
                .ToList();

        if (!filteredProducts.Any(p => p.Id == Guid.Empty))
            filteredProducts.Insert(0, new ProductInfo { Id = Guid.Empty, Designation = "Create Product" });
    }

    public void HideModal()
    {
        visibility = false;
        SelectedProductId = null;
        selectedProductModel = null;
        UnitPrice = 0;
        Quantity = 0;
        filterText = string.Empty;
        isProductSelected = false;
        StateHasChanged();
    }

    private async Task onProductSelected(object value)
    {
        if (value == null) return;

        if (Guid.TryParse(value.ToString(), out var productId))
        {
            if (productId == Guid.Empty)
            {
                SelectedProductId = null;
                await showProductForm();
            }
            else
            {
                SelectedProductId = productId;
                var product = products.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    selectedProductModel = new PurchaseProductModel
                    {
                        Id = product.Id,
                        Designation = product.Designation,
                        UnitMeasure = product.UnitMeasure,
                        TVA = product.TVA,
                        Quantity = product.Quantity
                    };

                    products.RemoveAll(p => p.Id == product.Id);
                    filteredProducts.RemoveAll(p => p.Id == product.Id);
                    isProductSelected = true;
                }
            }

            StateHasChanged();
        }
    }

    private async Task onSelectClicked()
    {
        if (selectedProductModel != null)
        {
            selectedProductModel.UnitPrice = UnitPrice;
            selectedProductModel.Quantity = Quantity;
            HideModal();
        }
    }

    private async Task Save()
    {
        if (selectedProductModel != null)
        {
            selectedProductModel.UnitPrice = UnitPrice;
            selectedProductModel.Quantity = Quantity;

            products.RemoveAll(p => p.Id == selectedProductModel.Id);
            filteredProducts.RemoveAll(p => p.Id == selectedProductModel.Id);

            await OnProductSelected.InvokeAsync(selectedProductModel);
            HideModal();
        }
    }

    private async Task onProductCreated(ProductInfo product)
    {
        products.Add(product);
        filteredProducts.Add(product);
        isProductSelected = true;
        ShowModal(product);
    }

    private async Task showProductForm()
    {
        productForm.ShowModal();
        HideModal();
        StateHasChanged();
    }
}