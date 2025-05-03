using INV.App.Products;
using INV.Domain.Entities.Products;
using INV.Shared;
using INVUIs.Products;
using INVUIs.Products.ProductsModel;
using INVUIs.Purchases.PurchaseModels;
using INVUIs.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace INV.Web.Components.Pages.Products;

public partial class ProductDetailPage :ComponentBase
{
    [Parameter] public Guid ProductId { get; set; }
    public ProductForm ProductForm;
    private ProductDetail? product;
    private FormState formState = FormState.Edit;

    protected override async Task OnInitializedAsync()
    {
        var result = await ProductService.GetProductById(ProductId);
        if (result.IsSuccess)
        {
            product = result.Value;
        }
        StateHasChanged();
    }

    private void ShowProductFormModal(ProductDetail product)
    {
        ProductForm.productModel = new ProductModel
        {
            ID = product.Id,
            Designation = product.Designation,
            UnitMeasure = product.UnitMeasure,
            TVA = product.TVA,
            WareHouseId = product.DefaultWareHouseId
        };
        ProductForm.formState = FormState.Edit;
        ProductForm.ShowModal();
    }
}