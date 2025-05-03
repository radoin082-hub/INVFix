using INV.App.Purchases;
using INV.App.Receipts;
using INV.App.Suppliers;
using INV.Domain.Entities.Receipts;
using INV.Web.Services.Suppliers;
using INVUIs.Suppliers;
using INVUIs.Suppliers.Models;
using Microsoft.AspNetCore.Components;

namespace INV.Web.Components.Pages.Suppliers
{
    public partial class SupplierPage :ComponentBase
    {
        [Parameter] public Guid id { get; set; }
        [Inject] public IAppSupplierService serviceSupplier { set; get; }
        [Inject] public IPurchaseOrderService purchaseOrderService { get; set; }
        [Inject] public IReceiptService receiptService { get; set; }
        private SupplierDetail supplier { get; set; }
        private List<PurchaseOrderInfo> purchases;
        private List<ReceiptInfo> receptions;

        private SupplierForm supplierForm = new SupplierForm();
     

        protected override async Task OnInitializedAsync()
        {
            await LoadSupplierData();
        }

        private async Task LoadSupplierData()
        {
            supplier = await serviceSupplier.GetSupplierDetail(id);
            var result = await purchaseOrderService.GetPurchaseOrdersByIdSupplier(id);
            if (result.IsSuccess)
            {
                purchases = result.Value;
            }

            var recepipt = await receiptService.GetReceiptsBySupplierId(id);
            if (recepipt.IsSuccess)
            {
                receptions = recepipt.Value.ToList();
            }
        }

        private void editSupplier()
        {
            supplierForm.SupplierToEdit = new SupplierModel
            {
                ID = supplier.Id,
                NameSupplier = supplier.ManagerName,
                NameCompany = supplier.CompanyName,
                Email = supplier.Email,
                Address = supplier.Address,
                Phone = supplier.Phone,
                ART = supplier.ART,
                NIF = supplier.NIF,
                RC = supplier.RC,
                NIS = supplier.NIS,
                RIB = supplier.RIB,
                BankAgency = supplier.BankAgency
            };
            supplierForm.Update = true;
            supplierForm.ShowModal();
        }

        private async Task OnSupplierSaved(SupplierInfo updatedSupplier)
        {
            await LoadSupplierData();
        }
    }
}