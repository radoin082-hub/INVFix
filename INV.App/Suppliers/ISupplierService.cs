using INV.Domain.Entities.Suppliers;
using INV.Domain.Shared;

namespace INV.App.Suppliers
{
    public interface ISupplierService
    {
        ValueTask<Result> AddSupplier(Supplier supplier);

        ValueTask<Result<List<SupplierInfo>>> GetAllSupplier();

        ValueTask<Result<ISupplier>> GetSupplierById(Guid id);

        ValueTask<Result> SetSupplier(Supplier supplier);

        ValueTask<Result> RemoveSupplierById(Guid id);

        ValueTask<int> GetSupplierCountAsync();
    }
}