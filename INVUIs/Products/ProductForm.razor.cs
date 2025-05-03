using INV.App.Products;
using INV.App.WareHouses;
using INV.Domain.Entities.Products;
using INV.Domain.Entities.WareHouses;
using INV.Domain.Shared;
using INV.Shared;
using INVUIs.Products.ProductsModel;
using INVUIs.Purchases.PurchaseModels;
using INVUIs.Shared.Models;
using INVUIs.Shared.MyAlert;
using INVUIs.WareHouses.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace INVUIs.Products;

public partial class ProductForm : ComponentBase
{
    [Parameter] public EventCallback<ProductInfo> OnProductCreated { get; set; }
    [Parameter] public ProductDetail productEdit { get; set; }
    [Parameter] public bool ShowToast { get; set; } = true;

    [Parameter] public RenderFragment Pills { get; set; }
    [Inject] private IProductService productService { set; get; }
    [Inject] private NavigationManager navigationManager { set; get; }
    [Inject] private IJSRuntime jsRuntime { set; get; }
    [Inject] IWareHouseService wareHouseService { get; set; } 

    public ProductForm productForm;

    public FormState formState;
    public ProductModel productModel = new ProductModel();
    private MyAlert? myAlert;
    private Result result;
    private string message = string.Empty;
    private List<int> TVAOptions = new() { 9, 19 };

    /*private List<WareHouseModel> wareHouse;*/
    private List<WareHouse> wareHouses;
    private List<string> UnitMesures = new() { "U", "KG", "M", "L" };
    private bool visibility = false;
    private string succesMessage => formState == FormState.Create ? Localizer["Product.Created.Title"] : Localizer["Product.Edited.Title"];

    protected override async Task OnInitializedAsync()
    {
        myAlert = new MyAlert(jsRuntime);
        var result = await wareHouseService.GetAllWareHousesByWarhouseType();
        if (result.IsSuccess)
        {
            wareHouses = result.Value;
        }
    }


    public async Task SubmitProduct()
    {
        var product = new ProductInfo
        {
            Id = productModel.ID,
            UnitMeasure = productModel.UnitMeasure,
            Designation = productModel.Designation,
            TVA = productModel.TVA,
            DefaultWareHouseId = productModel.WareHouseId
        };
        if (formState == FormState.Create)
        {
            var productadd = new Product
            {
                Id = productModel.ID,
                UnitMeasure = productModel.UnitMeasure,
                Designation = productModel.Designation,
                TVA = productModel.TVA,
                DefaultWareHouseId = productModel.WareHouseId,
            };
            result = await productService.CreateProduct(productadd);
            Hide();
            if (ShowToast)
                await myAlert.ShowToast(succesMessage, Localizer["Product.Created"], MyAlertType.success);
        }
        else
        {
            var productupdate = new Product
            {
                Id = productModel.ID,
                UnitMeasure = productModel.UnitMeasure,
                Designation = productModel.Designation,
                TVA = productModel.TVA,
                DefaultWareHouseId = productModel.WareHouseId
            };

            result = await productService.SetProducts(productupdate);

            Hide();
            await myAlert.ShowToast(succesMessage, Localizer["Product.Edited"], MyAlertType.success);
            navigationManager.NavigateTo(navigationManager.Uri, forceLoad: true);
        }

        if (result.IsSuccess)
        {
            await OnProductCreated.InvokeAsync(product);
            Hide();
        }
        else message = result.Error.Description;
    }

    private void clearForm()
    {
        productModel = new();
        message = string.Empty;
    }

    public void ShowModal()
    {
        visibility = true;
        StateHasChanged();
    }

    public void Hide()
    {
        clearForm();
        visibility = false;
        StateHasChanged();
    }
}