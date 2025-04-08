using INV.App.Suppliers;
using INV.Domain.Entities.Suppliers;
using INV.Domain.Shared;
using INV.Infrastructure.Storage.Products;
using INV.Infrastructure.Storage.SupplierStorages;

namespace INV.Implementation.Service.Suppliers
{
    public class SupplierService(ISupplierStorage supplierStorage) : ISupplierService
    {
        public async ValueTask<Result> AddSupplier(Supplier supplier)
        {
            try
            {
                List<Error> errorList = await validateSupplierCreate(supplier);

                if (errorList.Any())
                    return Result.Failure(errorList);

                await supplierStorage.InsertSupplier(supplier);
                return Result.Success();
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask<Result<List<SupplierInfo>>> GetAllSupplier()
        {
            try
            {
                List<Supplier> result = await supplierStorage.SelectAllSupplier();

                return Result.Success(result.Select(s => new SupplierInfo()
                {
                    ID = s.Id,
                    Name = s.ManagerName,
                    Address = s.Address,
                    Phone = s.Phone,
                    Email = s.Email,
                    CompanyName = s.CompanyName
                }).ToList());
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask<Result<ISupplier>> GetSupplierById(Guid id)
        {
            try
            {
                var result = await supplierStorage.SelectSupplierById(id);
                return result;
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask<Result> SetSupplier(Supplier supplier)
        {
            try
            {
                await supplierStorage.UpdateSupplier(supplier);
                return Result.Success();
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        private async Task<List<Error>> validateSupplierCreate(Supplier supplier)
        {
            List<Error> errors = new List<Error>();

            bool rcExists = await supplierStorage.SupplierExistsByRC(supplier.RC);
            if (rcExists)
                errors.Add(SupplierError.RCExsist(supplier.RC));

            bool nisExists = await supplierStorage.SupplierExistsByNIS(supplier.NIS);
            if (nisExists)
                errors.Add(SupplierError.NISExsist);
            bool ribExists = await supplierStorage.SupplierExistsByRIB(supplier.RIB);
            if (ribExists)
                errors.Add(SupplierError.RIBExsist);

            return errors;
        }

        public async ValueTask<Result> RemoveSupplierById(Guid id)
        {
            try
            {
                bool purchaseCount = await supplierStorage.SelectPurchaseCountBySupplierId(id);
                if (purchaseCount)
                    return Error.Failure("ErrorDelete", "The supplier has purchases.");
                await supplierStorage.DeleteSupplierById(id);
                return Result.Success();
            }
            catch (Exception e)
            {
                return Error.Exception(e);
            }
        }

        public async ValueTask<int> GetSupplierCountAsync()
        {
            return await supplierStorage.SelectSupplierCount();
        }
    }
}