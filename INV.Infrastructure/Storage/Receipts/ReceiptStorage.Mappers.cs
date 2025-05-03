using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INV.App.Purchases;
using INV.App.Receipts;
using INV.App.Suppliers;
using INV.Domain.Entities.Budget;
using INV.Domain.Entities.Receipts;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Types;

namespace INV.Infrastructure.Storage.Receipts
{
    public partial class ReceiptStorage
    {
        private static Receipt GetReceiptData(SqlDataReader reader)
        {
            return new Receipt
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                PurchaseId = reader.GetGuid(reader.GetOrdinal("PurchaseId")),
                Date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("Date"))),
                DeliveryNumber = reader.GetString(reader.GetOrdinal("DeliveryNumber")),
                DeliveryDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DeliveryDate"))),
                Status = (ReceiptStatus)reader.GetInt32(reader.GetOrdinal("Status"))
            };
        }

        private static ReceiptInfo getReceiptInfoFromReader(SqlDataReader reader)
        {
            return new ReceiptInfo
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                Number = reader.IsDBNull(reader.GetOrdinal("Number"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Number")),
                PurchaseId = reader.GetGuid(reader.GetOrdinal("PurchaseId")),
                purchaseNumber = reader.GetString(reader.GetOrdinal("PurchaseNumber")),
                PurchaseDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("PurchaseDate"))),
                Date = reader.IsDBNull("Date")
                    ? default
                    : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("Date"))),
                DeliveryNumber = reader.IsDBNull("DeliveryNumber")
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("DeliveryNumber")),
                DeliveryDate = reader.IsDBNull("DeliveryDate")
                    ? default
                    : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DeliveryDate"))),
                supplierId = reader.GetGuid(reader.GetOrdinal("SupplierId")),
                supplierName = reader.GetString(reader.GetOrdinal("SupplierName")),
                Status = (ReceiptStatus)reader.GetInt32(reader.GetOrdinal("Status"))
            };
        }

        private static ReceiptProduct GetReceiptProductData(SqlDataReader reader)
        {
            return new ReceiptProduct
            {
                ReceptionId = reader.GetGuid(reader.GetOrdinal("ReceptionId")),
                ProductId = reader.GetGuid(reader.GetOrdinal("ProductId")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
            };
        }

        private static ReceiptProductInfo GetReceiptProductInfoDataFromDataRow(DataRow row)
        {
            return new ReceiptProductInfo()
            {
                ReceptionId = (Guid)row["ReceptionId"],
                ProductId = (Guid)row["ProductId"],
                Quantity = (int)row["Quantity"],
                Designation = (string)row["Designation"],
            };
        }

        private static ReceiptInfo GetReceiptInfoFromDataRow(DataRow row)
        {
            return new ReceiptInfo()
            {
                Id = (Guid)row["Id"],
                Number = row.IsNull("Number") ? null : (string)row["Number"],
                PurchaseId = (Guid)row["PurchaseId"],
                Date = row.IsNull("Date") ? (DateOnly?)null : DateOnly.FromDateTime((DateTime)row["Date"]),
                purchaseNumber = row.IsNull("PurchaseNumber") ? null : (string)row["PurchaseNumber"],
                supplierId = (Guid)row["supplierId"],
                supplierName = row.IsNull("supplierName") ? null : (string)row["supplierName"],
                DeliveryNumber = row.IsNull("DeliveryNumber") ? null : (string)row["DeliveryNumber"],
                DeliveryDate = row.IsNull("DeliveryDate")
                    ? (DateOnly?)null
                    : DateOnly.FromDateTime((DateTime)row["DeliveryDate"]),
                Status = (ReceiptStatus)row["Status"]
            };
        }

        private static ReceiptDetail GetReceiptDetailFromDataRow(DataRow row)
        {
            return new ReceiptDetail()
            {
                Id = (Guid)row["Id"],
                Number = row.IsNull("Number") ? null : (string)row["Number"],
                PurchaseId = (Guid)row["PurchaseId"],
                Date = row.IsNull("Date") ? (DateOnly?)null : DateOnly.FromDateTime((DateTime)row["Date"]),
                purchaseNumber = row.IsNull("PurchaseNumber") ? null : (string)row["PurchaseNumber"],
                supplierId = (Guid)row["supplierId"],
                supplierName = row.IsNull("supplierName") ? null : (string)row["supplierName"],
                DeliveryNumber = row.IsNull("DeliveryNumber") ? null : (string)row["DeliveryNumber"],
                DeliveryDate = row.IsNull("DeliveryDate")
                    ? (DateOnly?)null
                    : DateOnly.FromDateTime((DateTime)row["DeliveryDate"]),
                Status = (ReceiptStatus)row["Status"]
            };
        }

        private static ReceiptInfo GetReceiptDatabyidsupplier(SqlDataReader reader)
        {
            return new ReceiptInfo
            {
                Id = reader.GetGuid(reader.GetOrdinal("ReceptionId")),
                PurchaseId = reader.GetGuid(reader.GetOrdinal("PurchaseId")),
                Date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("ReceptionDate"))),
                DeliveryNumber = reader.GetString(reader.GetOrdinal("DeliveryNumber")),
                DeliveryDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DeliveryDate"))),
                Status = (ReceiptStatus)reader.GetInt32(reader.GetOrdinal("ReceptionStatus"))
            };
        }

        private ReceiptProductInfo GetReceiptProductInfoFromDataRow(DataRow row)
        {
            return new ReceiptProductInfo
            {
                ReceptionId = row.IsNull("ReceptionId") ? Guid.Empty : row.Field<Guid>("ReceptionId"),
                ProductId = row.IsNull("ProductId") ? Guid.Empty : row.Field<Guid>("ProductId"),
                Quantity = row.IsNull("Quantity") ? 0 : row.Field<int>("Quantity"),
                Designation = row.IsNull("Designation") ? string.Empty : row.Field<string>("Designation"),
                //UnitPrice = row.IsNull("UnitPrice") ? 0m : row.Field<decimal?>("UnitPrice") ?? 0m,
                Received = row.IsNull("Received") ? 0 : row.Field<int>("Received"),
                DefaultWareHouseId = row.IsNull("WareHouseId")
                    ? Guid.Empty
                    : row.Field<Guid>("WareHouseId"),
            };
        }

        private static ReceiptDetail getReceptionDetailReader(SqlDataReader reader)
        {
            return new ReceiptDetail
            {
                Id = (Guid)reader["Id"],
                PurchaseId = (Guid)reader["PurchaseId"],
                Date = DateOnly.FromDateTime((DateTime)reader["Date"]),
                DeliveryNumber = (string)reader["DeliveryNumber"],
                DeliveryDate = DateOnly.FromDateTime((DateTime)reader["DeliveryDate"]),
                Status = (ReceiptStatus)reader["Status"],
                PurchaseOrder = new PurchaseOrderInfo(),
                ReceiptProducts = new List<ReceiptProductInfo>()
            };
        }

        private static ReceiptProductInfo getReceiptProductInfoReader(SqlDataReader reader)
        {
            return new ReceiptProductInfo()
            {
                ReceptionId = (Guid)reader["ReceptionId"],
                ProductId = (Guid)reader["ProductId"],
                Designation = (string)reader["Designation"],
                UnitMeasure = (string)reader["UnitMeasure"],
                Quantity = (int)reader["Quantity"],
                Received = (int)reader["Received"],
            };
        }

        private static PurchaseOrderInfo getPurchaseOrderReader(SqlDataReader reader)
        {
            return new PurchaseOrderInfo
            {
                Id = (Guid)reader["Id"],
                Number = (string)reader["Number"],
                SupplierName = (string)reader["SupplierName"],
                Date = DateOnly.FromDateTime((DateTime)reader["Date"]),
                BudgeType = (BudgeType)reader["BudgetArticle"],
                ServiceType = (ServiceType)reader["ServiceType"],
                TotalTTC = (decimal)reader["TotalTC"],
                CompletionDelay = (int)reader["CompletionDelay"],
                Supplier = new SupplierInfo
                {
                    CompanyName = (string)reader["ManagerName"],
                    Address = (string)reader["Address"],
                    Phone = (string)reader["Phone"],
                    Email = (string)reader["Email"],
                }
            };
        }
    }
}