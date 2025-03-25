using INV.App.Purchases;
using INV.Domain.Entities.Budget;
using INV.Domain.Entities.Purchases;
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
                // BudgeArticle = (string)reader["BudgetArticle"],
                //BudgeType = (BudgeType)reader["BudgetType"],
                ServiceType = (ServiceType)reader["ServiceType"],
                TotalHT = (decimal)reader["TotalHT"],
                TotalTVA = (decimal)reader["TotalVA"],
                TotalTTC = (decimal)reader["TotalTC"],
                CompletionDelay = (int)reader["CompletionDelay"],
                VisaNumber = reader.IsDBNull(reader.GetOrdinal("VisaNumber")) ? null : reader["VisaNumber"].ToString(),
                VisaDate = reader.IsDBNull(reader.GetOrdinal("VisaDate"))
                    ? null
                    : DateOnly.FromDateTime((DateTime)reader["VisaDate"]),
                Status = (PurchaseStatus)reader["Status"]
            };
        }

        private static PurchaseOrderInfo getPurchaseOrdersInfoData(SqlDataReader reader)
        {
            var r = new PurchaseOrderInfo
            {
                Id = (Guid)reader["Id"],
                SupplierId = (Guid)reader["SupplierId"],
                Number = (string)reader["Number"],
                Status = (PurchaseStatus)reader["Status"],
                SupplierName = (string)reader["CompanyName"],
                Date = DateOnly.FromDateTime((DateTime)reader["Date"]),
                BudgeArticle = (string)reader["BudgetArticle"],
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
}
}