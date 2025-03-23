using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INV.App.Receipts;
using INV.Domain.Entities.Receipts;
using Microsoft.Data.SqlClient;

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
                Number = reader.IsDBNull(reader.GetOrdinal("Number")) ? null : reader.GetString(reader.GetOrdinal("Number")),
                PurchaseId = reader.GetGuid(reader.GetOrdinal("PurchaseId")),
                purchaseNumber = reader.GetString(reader.GetOrdinal("PurchaseNumber")),
                PurchaseDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("PurchaseDate"))),
                Date = reader.IsDBNull("Date") ? default : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("Date"))),
                DeliveryNumber = reader.IsDBNull("DeliveryNumber") ? string.Empty : reader.GetString(reader.GetOrdinal("DeliveryNumber")),
                DeliveryDate = reader.IsDBNull("DeliveryDate") ? default : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DeliveryDate"))),
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
                DeliveryDate = row.IsNull("DeliveryDate") ? (DateOnly?)null : DateOnly.FromDateTime((DateTime)row["DeliveryDate"]),
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
                DeliveryDate = row.IsNull("DeliveryDate") ? (DateOnly?)null : DateOnly.FromDateTime((DateTime)row["DeliveryDate"]),
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
    }
}