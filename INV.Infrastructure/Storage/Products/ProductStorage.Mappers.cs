using System.Data;
using INV.App.Products;
using INV.App.Receipts;
using INV.Domain.Entities.Receipts;
using Microsoft.Data.SqlClient;

namespace INV.Infrastructure.Storage.Products
{
    public partial class ProductStorage
    {
        private static ProductInfo getProductData(SqlDataReader reader)
        {
            return new ProductInfo
            {
                Id = (Guid)reader["Id"],
                Designation = (string)reader["Designation"],
                UnitMeasure = (string)reader["UnitMeasure"],
                Quantity = (int)reader["Quantity"],
                TVA = (int)reader["TVA"],
            };
        }

        private static ProductInfo MapToProductInfo(SqlDataReader reader)
        {
            return new ProductInfo
            {
                Id = (Guid)reader["Id"],
                Designation = (string)reader["Designation"],
                UnitMeasure = (string)reader["UnitMeasure"],
                Quantity = (int)reader["Quantity"],
                TVA = (int)reader["TVA"],
                WareHouse = reader["WarehouseName"] != DBNull.Value ? (string)reader["WarehouseName"] : null
            };
        }

        // Mapper pour ProductDetail et ReceiptInfo
        private static ProductDetail MapToProductDetail(DataSet dataSet)
        {
            if (dataSet.Tables[0].Rows.Count == 0)
                return null; // Aucun produit trouvé

            // Mappe les détails du produit à partir de la première table
            var productRow = dataSet.Tables[0].Rows[0];
            var product = new ProductDetail
            {
                Id = (Guid)productRow["Id"],
                Designation = productRow["Designation"].ToString(),
                UnitMeasure = productRow["UnitMeasure"].ToString(),
                Quantity = Convert.ToInt32(productRow["Quantity"]),
                TVA = Convert.ToInt32(productRow["TVA"]),
                DefaultWareHouseId = (Guid)productRow["DefaultWareHouseID"],
                WareHouse = productRow["WareHouse"].ToString()
            };

            // Mappe les réceptions à partir de la deuxième table (si présente)
            if (dataSet.Tables.Count > 1)
            {
                foreach (DataRow row in dataSet.Tables[1].Rows)
                {
                    var receipt = new ReceiptInfo
                    {
                        Id = (Guid)row["Id"],
                        Number = row.IsNull("Number") ? null : row["Number"].ToString(),
                        PurchaseId = (Guid)row["PurchaseId"],
                        Date = row.IsNull("Date") ? default : DateOnly.FromDateTime((DateTime)row["Date"]),
                        DeliveryNumber = row.IsNull("DeliveryNumber") ? string.Empty : row["DeliveryNumber"].ToString(),
                        DeliveryDate = row.IsNull("DeliveryDate") ? default : DateOnly.FromDateTime((DateTime)row["DeliveryDate"]),
                        Status = (ReceiptStatus)row["Status"],
                        supplierName = (string)row["CompanyName"],
                        Quantity = Convert.ToInt32(row["Quantity"]),
                    };
                    product.ReceiptInfos.Add(receipt);
                }
            }

            return product;
        }
    }
}