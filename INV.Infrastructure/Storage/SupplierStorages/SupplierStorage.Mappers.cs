using INV.Domain.Entities.Suppliers;
using Microsoft.Data.SqlClient;

namespace INV.Infrastructure.Storage.SupplierStorages
{
public partial class SupplierStorage
{
    private static Supplier getSupplierData(SqlDataReader reader)
    {
        var r = new Supplier
        {
            Id = (Guid)reader["Id"],
            CompanyName = (string)reader["CompanyName"],
            ManagerName = (string)reader["ManagerName"],
            Address = (string)reader["Address"],
            Phone = (string)reader["Phone"],
            Email = (string)reader["Email"],
            RC = (string)reader["RC"],
            NIS = (string)reader["NIS"],
            ART = (string)reader["ART"],
            RIB = (string)reader["RIB"],
            NIF = (string)reader["NIF"],
            BankAgency = (string)reader["BankAgency"],
            State = (SupplierState)reader["Status"]
        };
        return r;
    }
    
}
}