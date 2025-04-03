using INV.Domain.Entities.Suppliers;

namespace INV.Infrastructure.Storage.SupplierStorages
{
    public interface ISupplierStorage
    {
        Task<int> InsertSupplier(Supplier supplier);
        Task<List<Supplier>> SelectAllSupplier();
        Task<Supplier> SelectSupplierById(Guid id);
        Task<int> UpdateSupplier(Supplier supplier);
        Task<bool> SupplierExistsByRC(string rc);
        Task<bool> SupplierExistsByNIS(string nis);
        Task<bool> SupplierExistsByRIB(string rib);
        ValueTask<int> DeleteSupplierById(Guid id);

        ValueTask<int> SelectSupplierCount();
    }
}

