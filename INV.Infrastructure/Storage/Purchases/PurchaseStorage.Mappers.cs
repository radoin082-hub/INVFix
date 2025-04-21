using INV.App.Purchases;
using INV.Domain.Entities.Budget;
using INV.Domain.Entities.Products;
using INV.Domain.Entities.Purchases;
using INV.Domain.Entities.Suppliers;
using Microsoft.Data.SqlClient;

namespace INV.Infrastructure.Storage.Purchases
{
    public partial class PurchaseOrderStorage
    {
        private static PurchaseOrder getPurchaseOrdersData(SqlDataReader reader)
        {
            return new PurchaseOrder
            {
                Id = (Guid)reader["Id"],
                Number = (string)reader["Number"],
                SupplierId = (Guid)reader["SupplierId"],
                Date = DateOnly.FromDateTime((DateTime)reader["Date"]),
                BudgetArticle = (int)reader["BudgetArticle"],
                BudgetChapter = (int)reader["BudgetChapter"],
                BudgetType = (BudgeType)reader["BudgetType"],
                ServiceType = (ServiceType)reader["ServiceType"],
                TotalHT = (decimal)reader["TotalHT"],
                TotalTVA = (decimal)reader["TotalVA"],
                TotalTTC = (decimal)reader["TotalTC"],
                CompletionDelay = (int)reader["CompletionDelay"],
                VisaNumber = reader.IsDBNull(reader.GetOrdinal("VisaNumber")) ? null : reader["VisaNumber"].ToString(),
                VisaDate = reader.IsDBNull(reader.GetOrdinal("VisaDate"))
                    ? null
                    : DateOnly.FromDateTime((DateTime)reader["VisaDate"]),
                Observation = reader.IsDBNull(reader.GetOrdinal("Observation")) ? null : (string)reader["Observation"],

                Status = (PurchaseStatus)reader["Status"]
            };
        }

        private static PurchaseOrderInfo getPurchaseOrdersInfoData(SqlDataReader reader)
        {
            var r = new PurchaseOrderInfo
            {
                Id = (Guid)reader["Id"],
                SupplierId = (Guid)reader["SupplierId"],
                Number = reader["Number"] != DBNull.Value ? (string)reader["Number"] : null,
                Status = (PurchaseStatus)reader["Status"],
                SupplierName = reader["CompanyName"] != DBNull.Value ? (string)reader["CompanyName"] : null,
                Date = DateOnly.FromDateTime((DateTime)reader["Date"]),
                BudgeArticle = reader["BudgetArticle"] != DBNull.Value ? (string)reader["BudgetArticle"] : null,
                BudgeType = (BudgeType)reader["BudgetType"],
                ServiceType = (ServiceType)reader["ServiceType"],
                TotalTTC = (decimal)reader["TotalTC"]
            };
            return r;
        }

        private static PurchaseOrderInfo getPurchaseForcreationData(SqlDataReader reader)
        {
            var r = new PurchaseOrderInfo
            {
                Id = (Guid)reader["Id"],
                SupplierId = (Guid)reader["SupplierId"],
                Number = (string)reader["Number"],
                Status = (PurchaseStatus)reader["Status"],
                SupplierName = (string)reader["CompanyName"],
                Date = DateOnly.FromDateTime((DateTime)reader["Date"])
            };
            return r;
        }

        private static PurchaseProduct getPurchaseProductsData(SqlDataReader reader)
        {
            return new PurchaseProduct
            {
                ProductId = (Guid)reader["ProductId"],
                PurchaseOrderId = (Guid)reader["Id"],
                Quantity = (int)reader["Quantity"],
                UnitPrice = (decimal)reader["UnitPrice"],
            };
        }

        private static PurchaseDetail getPurchaseDetailData(SqlDataReader reader)
        {
            return new PurchaseDetail
            {
                Id = (Guid)reader["Id"],
                Number = (string)reader["Number"],
                SupplierId = (Guid)reader["SupplierId"],
                Date = DateOnly.FromDateTime((DateTime)reader["Date"]),
                BudgetChapter = (int)reader["BudgetChapter"],
                BudgetArticle = (int)reader["BudgetArticle"],
                BudgetType = (BudgeType)(int)reader["BudgetType"],
                ServiceType = (ServiceType)(int)reader["ServiceType"],
                TotalHT = (decimal)reader["TotalHT"],
                TotalTVA = (decimal)reader["TotalTVA"],
                TotalTTC = (decimal)reader["TotalTTC"],
                CompletionDelay = (int)reader["CompletionDelay"],
                Supplier = new Supplier
                {
                    Id = (Guid)reader["SupplierId"],
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
                    State = (SupplierState)(int)reader["Status"]
                },
                Products = new List<Product>()
            };
        }

        private static Product getProductsData(SqlDataReader reader)
        {
            return new Product
            {
                Id = (Guid)reader["ProductId"],
                DefaultWareHouseId = (Guid)reader["DefaultWareHouseId"],
                Designation = (string)reader["Designation"],
                UnitMeasure = (string)reader["UnitMeasure"],
                Quantity = (int)reader["Quantity"],
                UnitPrice = (decimal)reader["UnitPrice"],
                TVA = (int)reader["TVA"],
                /*WareHouse = (string)reader["DefaultWareHouseId"]*/
            };
        }
    }
}