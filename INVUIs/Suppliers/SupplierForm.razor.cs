using INV.App.Suppliers;
using INV.Domain.Entities.Suppliers;
using INV.Domain.Shared;
using INVUIs.Shared.MyAlert;
using INVUIs.Suppliers.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace INVUIs.Suppliers
{
    public partial class SupplierForm : ComponentBase
    {
        [Parameter] public EventCallback<SupplierInfo> OnSave { get; set; }
        [Parameter] public EventCallback<SupplierInfo> OnSupplierCreated { get; set; }

        [Parameter] public bool Update { get; set; } = false;
        [Parameter] public string CreateButtonLabel { get; set; } = "Register";
        [Parameter] public SupplierModel SupplierToEdit { get; set; }
        [Inject] public ISupplierService SupplierService { get; set; }
        [Inject] public NavigationManager navigationManager { get; set; }
        [Inject] private IJSRuntime jsRuntime { set; get; }
        private SupplierModel newSupplier = new SupplierModel();
        private bool displayModal = false;
        private MyAlert? myAlert;
        private Result result;
        private string success = string.Empty;
        private string succesMessage;

        protected override async Task OnInitializedAsync()
        {
            myAlert = new MyAlert(jsRuntime);
        }

        private void close()
        {
            newSupplier = new SupplierModel();
            displayModal = false;
            StateHasChanged();
        }

        public void CloseModel()
        {
            newSupplier = new SupplierModel();
            displayModal = false;
            StateHasChanged();
        }

        public void ShowModal()
        {
            if (Update && SupplierToEdit != null)
            {
                newSupplier = SupplierToEdit;
            }
            displayModal = true;
            StateHasChanged();
        }

        private async Task OnCreate()
        {
            var sup = new Supplier()
            {
                Id = Update ? SupplierToEdit.ID : Guid.NewGuid(),
                ManagerName = newSupplier.NameSupplier,
                CompanyName = newSupplier.NameCompany,
                Email = newSupplier.Email,
                Address = newSupplier.Address,
                Phone = newSupplier.Phone,
                ART = newSupplier.ART,
                NIF = newSupplier.NIF,
                RC = newSupplier.RC,
                NIS = newSupplier.NIS,
                RIB = newSupplier.RIB,
                BankAgency = newSupplier.BankAgency,
                State = SupplierState.Active
            };

            if (Update)
            {
                var result = await SupplierService.SetSupplier(sup);
                close();
                await myAlert.ShowToast(succesMessage, "The Supplier has been Edited.", MyAlertType.success);
            }
            else
            {
                result = await SupplierService.AddSupplier(sup);
                close();
                await myAlert.ShowToast(succesMessage, "The Supplier has been created.", MyAlertType.success);
            }

            success = "The supplier has been " + (Update ? "updated" : "added") + " successfully";
            await ClearForm();
            close();
            var createdSupplierInfo = new SupplierInfo
            {
                ID = sup.Id,
                Name = sup.ManagerName,
                CompanyName = sup.CompanyName,
                Email = sup.Email,
                Address = sup.Address,
                Phone = sup.Phone,
                ART = sup.ART,
                NIF = sup.NIF,
                RC = sup.RC,
                NIS = sup.NIS,
                RIB = sup.RIB,
                BankAgency = sup.BankAgency
            };

            await OnSupplierCreated.InvokeAsync(createdSupplierInfo);
            // await OnSave.InvokeAsync(createdSupplierInfo);
            CloseModel();
            success = "The supplier has been added successfully";
            await ClearForm();
        }

        private async Task ClearForm()
        {
            newSupplier = new SupplierModel();
        }
    }
}