using System.Runtime.CompilerServices;
using INV.App.Products;
using INV.App.Purchases;
using INV.Domain.Entities.Purchases;
using INV.Implementation.Service.Products;
using INVUIs.Products;
using INVUIs.Products.ProductsModel;
using INVUIs.Purchases.PurchaseModels;
using INVUIs.Shared;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace INVUIs.Purchases;

public partial class PurchaseProducts : ComponentBase
{
    [CascadingParameter] public bool canEdit { get; set; }
    [CascadingParameter] public List<PurchaseProductModel> products { set; get; } = new();
    [Parameter] public EventCallback<List<PurchaseProductModel>> OnProductAddProduct { get; set; }
    [Parameter] public PurchaseOrder PurchaseInfo { get; set; }
    [Parameter] public EventCallback<PurchaseProductModel> OnEditProduct { get; set; }
    [Parameter] public List<PurchaseProductModel> ProductList { get; set; }
    [Inject] private IPurchaseOrderService purchaseOrderService { get; set; }
    private ConformationForm conformationForm;
    private List<int> TVAOptions = new() { 9, 19 };
    private List<string> UnitMesures = new() { "U", "KG", "M", "L" };
    public RadzenDataGrid<PurchaseProductModel> grid;
    private ProductEditForm productEditForm;
    private ProductForm productForm = new();
    public ProductSelector productSelector = new();
    private PurchaseProductModel? selectedProductModel = null;
    public ProductModel productModel { get; set; }

    private bool showEditPopup = false;
    private Guid ProductId;

    private bool StatusButton(PurchaseOrder purchaseOrder)
    {
        if (purchaseOrder == null || purchaseOrder.Status == PurchaseStatus.Editing)
        {
            return true;
        }
        return false;
    }

    protected override void OnParametersSet()
    {
        if (ProductList != null)
        {
            products = new List<PurchaseProductModel>(ProductList);
        }
    }

    private async Task DeleteProduct(PurchaseProductModel product)
    {
        ProductId = product.Id;
        conformationForm.show();
        StateHasChanged();
    }

    private async Task DeleteProductConformation()
    {
        var product = products.FirstOrDefault(p => p.Id == ProductId);
        if (product != null)
        {
            products.Remove(product);
            if (product.PurchaseOrderId != Guid.Empty)
            {
                await purchaseOrderService.RemovePurchaseProduct(product.Id, product.PurchaseOrderId);
            }
        }

        await grid.Reload();
        StateHasChanged();
    }

    private async Task AddProductToGrid(PurchaseProductModel product)
    {
        // product.Number = products.Count + 1;
        product.TotalPrice = product.Quantity * product.UnitPrice;
        if (PurchaseInfo is not null)
        {
            var purchaseProduct = new PurchaseProduct
            {
                ProductId = product.Id,
                PurchaseOrderId = PurchaseInfo.Id,
                Quantity = product.Quantity,
                UnitPrice = product.UnitPrice,
            };
            await purchaseOrderService.CreateProductPurchase(purchaseProduct);
            await grid.Reload();
        }
        else
        {
            var purchaseProduct = new PurchaseProduct
            {
                ProductId = product.Id,
                Quantity = product.Quantity,
                UnitPrice = product.UnitPrice,
            };
        }

        products.Add(product);
        StateHasChanged();
    }

    private async Task EditProduct(PurchaseProductModel product)
    {
        selectedProductModel = new PurchaseProductModel
        {
            Id = product.Id,
            PurchaseOrderId = product.PurchaseOrderId,
            Designation = product.Designation,
            UnitMeasure = product.UnitMeasure,
            Quantity = product.Quantity,
            UnitPrice = product.UnitPrice,
            TVA = product.TVA
        };
        productEditForm.show();
        StateHasChanged();
    }

    private async Task SaveEditedProduct()
    {
        if (selectedProductModel != null)
        {
            var product = products.FirstOrDefault(p => p.Designation == selectedProductModel.Designation);
            if (product != null)
            {
                product.Quantity = selectedProductModel.Quantity;
                product.UnitPrice = selectedProductModel.UnitPrice;
                product.TotalPrice = product.Quantity * product.UnitPrice;
            }
            showEditPopup = false;
            var purchaseProduct = new PurchaseProduct
            {
                ProductId = product.Id,
                PurchaseOrderId = product.PurchaseOrderId,
                Quantity = product.Quantity,
                UnitPrice = product.UnitPrice,
            };
            await purchaseOrderService.UpdatePurchaseProduct(purchaseProduct);
            StateHasChanged();
        }
    }

    public async void loadtab()
    {
        var product = products.FirstOrDefault(p => p.Designation == selectedProductModel.Designation);
        if (product != null)
        {
            product.Quantity = selectedProductModel.Quantity;
            product.UnitPrice = selectedProductModel.UnitPrice;
            product.TotalPrice = product.Quantity * product.UnitPrice;
        }
        await grid.Reload();
    }

    private decimal getTHT()
    {
        return products.Sum(p => p.UnitPrice * p.Quantity);
    }

    private decimal getTVA()
    {
        return products.Sum(p => p.UnitPrice * p.Quantity * p.TVA) / 100;
    }

    private decimal getTTC()
    {
        return getTHT() + getTVA();
    }
}