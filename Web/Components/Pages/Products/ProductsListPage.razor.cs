using INV.App.Products;
using INV.Domain.Entities.Products;
using Microsoft.AspNetCore.Components;

namespace INV.Web.Components.Pages.Products;

public partial class ProductsListPage :ComponentBase
{
    [Inject] public IProductService productService { get; set; }
    private List<ProductInfo> products = new List<ProductInfo>();

    protected override async Task OnInitializedAsync()
    {
        var result = await productService.GetProducts();
        if (result.IsSuccess)
        {
            products = result.Value;
        }

        StateHasChanged();
    }
}