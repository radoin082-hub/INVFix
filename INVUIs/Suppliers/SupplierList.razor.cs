using BlazorBootstrap;
using INV.App.Suppliers;
using INV.Domain.Entities.Products;
using INV.Domain.Entities.Purchases;
using INV.Domain.Entities.Suppliers;
using INV.Implementation.Service.Products;
using INV.Implementation.Service.Purchses;
using INVUIs.Shared;
using INVUIs.Shared.MyAlert;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen.Blazor;
using Xunit.Sdk;

namespace INVUIs.Suppliers
{
    public partial class SupplierList
    {
        [Inject] private ISupplierService supplierService { get; set; }
        [Parameter] public List<SupplierInfo> Suppliers { get; set; } = new();
        [Inject] private NavigationManager navigationManager { get; set; }
        [Inject] private IJSRuntime jsRuntime { set; get; }
        public List<SupplierInfo> supplierFilter { get; set; } = new();
        public RadzenDataGrid<SupplierInfo> grid;
        public SupplierInfo supplierDelete;
        private ConformationForm conformationForm;
        private string _searchName = "";
        private Guid supplierId;
        private string errorMessage;
        private MyAlert? myAlert;

        private void NavigateToSupplierDetails(Guid supplierId)
        {
            navigationManager.NavigateTo($"/suppliers/{supplierId}");
        }

        protected override async Task OnInitializedAsync()
        {
            myAlert = new MyAlert(jsRuntime);
        }

        protected override void OnParametersSet()
        {
            supplierFilter = Suppliers;
        }

        private SupplierForm supplierForm;

        private void showSupplierForm()
        {
            if (supplierForm != null)
            {
                supplierForm.ShowModal();
            }
        }

        private string searchName
        {
            get => _searchName;
            set
            {
                _searchName = value;
                getByName();
            }
        }

        private void getByName()
        {
            if (string.IsNullOrWhiteSpace(searchName))
            {
                supplierFilter = Suppliers.ToList();
                return;
            }

            string searchLower = searchName.Trim().ToLower();

            supplierFilter = Suppliers
                .Where(s => (s.Name?.ToLower().Contains(searchLower) ?? false) ||
                            (s.Email?.ToLower().Contains(searchLower) ?? false) ||
                            (s.CompanyName?.ToLower().Contains(searchLower) ?? false) ||
                            (s.AccountName?.ToLower().Contains(searchLower) ?? false) ||
                            (s.Phone?.ToLower().Contains(searchLower) ?? false) ||
                            (s.Address?.ToLower().Contains(searchLower) ?? false))

                .OrderBy(s => s.Name)
                .ToList();

            StateHasChanged();
        }

        public void NavigatePage()
        {
            navigationManager.NavigateTo("Supplier");
        }

        public void ShowModal()
        {
            StateHasChanged();
        }

        private async Task DeleteSupplier(SupplierInfo Suppliers)
        {
            supplierId = Suppliers.ID;
            var result = await supplierService.GetPurchaseCountBySupplierId(supplierId);
            if (result)
            {
                await myAlert.ShowAlert(Localizer["ErrorDelete"], errorMessage, MyAlertType.error);
            }
            else
            {
                conformationForm.show();
            }
            StateHasChanged();
        }

        private async Task ConfirmDeleteSupplier()
        {
            /* await supplierService.RemoveSupplierById(supplierId);

             StateHasChanged();
 */
            supplierFilter = Suppliers;
            var supplierToRemove = supplierFilter.Find(p => p.ID == supplierId);

            if (supplierId != null)
            {
                var result = await supplierService.RemoveSupplierById(supplierToRemove.ID);
                if (result.IsSuccess)
                {
                    supplierFilter.Remove(supplierToRemove);
                    await grid.Reload();
                    await myAlert.ShowAlert(Localizer["DeleteSuccessfully"], Localizer["Supplier.Deleted"], MyAlertType.success);
                }

                StateHasChanged();
            }
        }
    }
}