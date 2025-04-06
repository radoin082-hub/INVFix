using BlazorBootstrap;
using INV.App.Suppliers;
using INV.Domain.Entities.Products;
using INV.Domain.Entities.Purchases;
using INV.Domain.Entities.Suppliers;
using INV.Implementation.Service.Products;
using INV.Implementation.Service.Purchses;
using INVUIs.Shared;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace INVUIs.Suppliers
{
    public partial class SupplierList
    {
        [Inject] private ISupplierService supplierService { get; set; }
        [Parameter] public List<SupplierInfo> Suppliers { get; set; } = new();
        [Inject] private NavigationManager navigationManager { get; set; }
        public List<SupplierInfo> supplierFilter { get; set; } = new();
        private RadzenDataGrid<SupplierInfo> grid;
        public SupplierInfo supplierDelete;
        private ConformationForm conformationForm;
        private string _searchName = "";
        private Guid supplierId;

        private void NavigateToSupplierDetails(Guid supplierId)
        {
            navigationManager.NavigateTo($"/suppliers/{supplierId}");
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
                .Where(s => s.Name.ToLower().Contains(searchLower) || s.Email.ToLower().Contains(searchLower))
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
            conformationForm.show();
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
                }
                await grid.Reload();
                StateHasChanged();
            }
        }
    }
}