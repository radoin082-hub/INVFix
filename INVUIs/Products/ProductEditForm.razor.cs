using INV.App.Purchases;
using INV.Domain.Entities.Purchases;
using INVUIs.Purchases.PurchaseModels;
using Microsoft.AspNetCore.Components;

namespace INVUIs.Products;

public partial class ProductEditForm
{
    [Parameter] public PurchaseProductInfo product { get; set; }
    [Parameter] public PurchaseProductModel selectedProductModel { get; set; }
    [Parameter] public EventCallback<PurchaseProductModel> productEdit { get; set; }
    [Inject] public IPurchaseOrderService purchaseOrderService { get; set; }
    public bool display = false;
    private PurchaseProduct editproduct;

    public void show()
    {
        display = true;
        StateHasChanged();
    }

    public void CloseEditPopup()
    {
        display = false;
        StateHasChanged();
    }

    public async Task SaveEditedProduct(PurchaseProductModel product)
    {
        if (product.PurchaseOrderId != Guid.Empty)
        {
            editproduct = new PurchaseProduct
            {
                ProductId = product.Id,
                PurchaseOrderId = product.PurchaseOrderId!,
                Quantity = product.Quantity,
                UnitPrice = product.UnitPrice,
            };
            await purchaseOrderService.UpdatePurchaseProduct(editproduct);
        }
        else
        {
            product.Quantity = selectedProductModel.Quantity;
            product.UnitPrice = selectedProductModel.UnitPrice;
            product.TotalPrice = product.Quantity * product.UnitPrice;
        }

        await productEdit.InvokeAsync(product);

        CloseEditPopup();
    }
}