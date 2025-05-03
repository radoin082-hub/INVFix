using System;
using System.Data;
using BootstrapBlazor.Components;
using INV.App.Receipts;
using INV.Domain.Entities.Budget;
using INV.Domain.Entities.Purchases;
using INV.Domain.Entities.Receipts;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace INV.Infrastructure.Storage.Receipts
{
    public partial class ReceiptStorage : IReceiptStorage
    {
        private readonly string _connectionString;

        public ReceiptStorage(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("INV");
        }

        private const string selectAllReceiptsQuery = "SELECT * FROM reception.GetList()";
        private const string selectReceiptByIdQuery = "SELECT * FROM [reception].[HEADERS] WHERE Id = @aId";
        private const string selectReceiptsByPurchaseIdQuery = "SELECT * FROM [reception].[HEADERS] WHERE PurchaseId = @aPurchaseId";
        private const string selectReceiptsByPurchaseIdQueryWhenStatus1 = "SELECT * FROM reception.GetList() WHERE PurchaseId = @aPurchaseId and Status=1";

        private const string insertReceiptCommand = @"
            INSERT INTO [reception].[HEADERS] (Id, PurchaseId, Date, DeliveryNumber, DeliveryDate, Status,Number)
            VALUES (@aId, @aPurchaseId, @aDate, @aDeliveryNumber, @aDeliveryDate, @aStatus,@aNumber)";

        private const string updateReceiptCommand = @"
            UPDATE [reception].[HEADERS]
            SET PurchaseId = @aPurchaseId, Date = @aDate, DeliveryNumber = @aDeliveryNumber,
                DeliveryDate = @aDeliveryDate, Status = @aStatus
            WHERE Id = @aId";

        private const string deleteReceiptCommand = "DELETE FROM [reception].[HEADERS] WHERE Id = @aId";

        // SQL Queries for ReceiptProduct
        private const string selectAllReceiptProductsQuery = "SELECT * FROM [reception].[PRODUCTS]";

        private const string selectProductsByReceptionIdQuery = "SELECT * FROM [reception].[PRODUCTS] WHERE ReceptionId = @aReceptionId";

        private const string insertReceiptProductCommand = @"
            INSERT INTO [reception].[PRODUCTS] (ReceptionId, ProductId, Quantity, WareHouseId)
            VALUES (@aReceptionId, @aProductId, @aQuantity,@aWareHouseId)";

        private const string updateReceiptProductCommand = @"
            UPDATE [reception].[PRODUCTS]
            SET Quantity = @aQuantity 
            ,   WareHouseId=@aWareHouseId
            WHERE ReceptionId = @aReceptionId AND ProductId = @aProductId";

        private const string deleteReceiptProductCommand = @"
            DELETE FROM [reception].[PRODUCTS] WHERE ReceptionId = @aReceptionId AND ProductId = @aProductId";

        private const string getReceptionDetailsProcedure = "dbo.GetReceptionDetails";
        private const string CreateReceiptFromPurchaseCommand = "reception.CreateFromPurchase";
        private const string getReceiptInfoById = "[reception].[GetById]";

        private const string selectReceiptsBySupplierIdQuery = @"SELECT
    H.Id AS ReceptionId,
    H.PurchaseId,
    H.Date AS ReceptionDate,
    H.DeliveryNumber,
    H.DeliveryDate,
    H.Status AS ReceptionStatus
FROM [INV].[reception].[HEADERS] H
JOIN [INV].[purchase].[ORDERS] O ON H.PurchaseId = O.Id
WHERE O.SupplierId = @aSupplierId;";

        private const string countofReceptions = @"SELECT COUNT(*)  FROM [reception].[HEADERS];";

        private const string countofStatusReception = @"SELECT COUNT(h.Status) AS Count
            FROM (SELECT 0 AS Status UNION ALL SELECT 1) AS s
            LEFT JOIN [reception].[HEADERS] AS h  ON h.Status = s.Status AND YEAR(h.Date) = YEAR(GETDATE()) 
            GROUP BY s.Status";
        private const string selectnewnumberReception =@" SELECT MAX(CAST([Number] AS BIGINT)) + 1 AS NextNumber 
            FROM[INV].reception.HEADERS WHERE ISNUMERIC([Number]) = 1";

        private const string setNumberReception = @"UPDATE [INV].reception.HEADERS
                                    SET [Number] = @aNumber WHERE [Id] = @aId;";
        public async ValueTask<ReceiptDetail> CreateReceiptFromPurchase(Guid purchaseId)
        {
            using var connection = new SqlConnection(_connectionString);

            using var cmd = new SqlCommand(CreateReceiptFromPurchaseCommand, connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@aPurchaseId", purchaseId);
            DataSet ds = new();
            SqlDataAdapter da = new(cmd);
            await connection.OpenAsync();
            da.Fill(ds);
            return GetReceiptFromDataSet(ds, true);
        }

        public async ValueTask<List<ReceiptInfo>> SelectAllReceipts()
        {
            await using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(selectAllReceiptsQuery, sqlConnection);
            await sqlConnection.OpenAsync();

            var reader = await cmd.ExecuteReaderAsync();
            var receipts = new List<ReceiptInfo>();
            while (await reader.ReadAsync())
            {
                receipts.Add(getReceiptInfoFromReader(reader));
            }

            return receipts;
        }

        public async ValueTask<Receipt?> SelectReceiptById(Guid id)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(selectReceiptByIdQuery, sqlConnection);
            cmd.Parameters.AddWithValue("@aId", id);
            await sqlConnection.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? GetReceiptData(reader) : null;
        }

        public async ValueTask<List<Receipt>> SelectReceiptsByPurchaseId(Guid purchaseId)
        {
            var receipts = new List<Receipt>();
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(selectReceiptsByPurchaseIdQuery, sqlConnection);
            cmd.Parameters.AddWithValue("@aPurchaseId", purchaseId);
            await sqlConnection.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                receipts.Add(GetReceiptData(reader));
            }

            return receipts;
        }

        public async ValueTask<List<ReceiptInfo>> SelectReceiptsByPurchaseIdWhenStatus1(Guid purchaseId)
        {
            var receipts = new List<ReceiptInfo>();
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(selectReceiptsByPurchaseIdQueryWhenStatus1, sqlConnection);
            cmd.Parameters.AddWithValue("@aPurchaseId", purchaseId);
            await sqlConnection.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                receipts.Add(getReceiptInfoFromReader(reader));
            }

            return receipts;
        }

        public async ValueTask<int> InsertReceipt(Receipt receipt)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(insertReceiptCommand, sqlConnection);
            await sqlConnection.OpenAsync();

            cmd.Parameters.AddWithValue("@aId", receipt.Id);
            cmd.Parameters.AddWithValue("@aPurchaseId", receipt.PurchaseId);
            cmd.Parameters.AddWithValue("@aDate", receipt.Date);
            cmd.Parameters.AddWithValue("@aDeliveryNumber", receipt.DeliveryNumber);
            cmd.Parameters.AddWithValue("@aDeliveryDate", receipt.DeliveryDate);
            cmd.Parameters.AddWithValue("@aStatus", receipt.Status);
            cmd.Parameters.AddWithValue("@aNumber", "0");

            return await cmd.ExecuteNonQueryAsync();
        }

        public async ValueTask<int> UpdateReceipt(Receipt receipt)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(updateReceiptCommand, sqlConnection);
            await sqlConnection.OpenAsync();

            cmd.Parameters.AddWithValue("@aId", receipt.Id);
            cmd.Parameters.AddWithValue("@aPurchaseId", receipt.PurchaseId);
            cmd.Parameters.AddWithValue("@aDate", receipt.Date);
            cmd.Parameters.AddWithValue("@aDeliveryNumber", receipt.DeliveryNumber);
            cmd.Parameters.AddWithValue("@aDeliveryDate", receipt.DeliveryDate);
            cmd.Parameters.AddWithValue("@aStatus", receipt.Status);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async ValueTask<int> DeleteReceipt(Guid id)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(deleteReceiptCommand, sqlConnection);
            await sqlConnection.OpenAsync();

            cmd.Parameters.AddWithValue("@aId", id);
            return await cmd.ExecuteNonQueryAsync();
        }

        public async ValueTask<List<ReceiptProduct>> SelectAllReceiptProducts()
        {
            var receiptProducts = new List<ReceiptProduct>();
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(selectAllReceiptProductsQuery, sqlConnection);
            await sqlConnection.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                receiptProducts.Add(GetReceiptProductData(reader));
            }

            return receiptProducts;
        }

        public async ValueTask<List<ReceiptProduct>> SelectProductsByReceptionId(Guid receptionId)
        {
            var receiptProducts = new List<ReceiptProduct>();
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(selectProductsByReceptionIdQuery, sqlConnection);
            cmd.Parameters.AddWithValue("@aReceptionId", receptionId);
            await sqlConnection.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                receiptProducts.Add(GetReceiptProductData(reader));
            }

            return receiptProducts;
        }

        public async ValueTask<int> InsertReceiptProduct(ReceiptProduct receiptProduct)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(insertReceiptProductCommand, sqlConnection);
            await sqlConnection.OpenAsync();

            cmd.Parameters.AddWithValue("@aReceptionId", receiptProduct.ReceptionId);
            cmd.Parameters.AddWithValue("@aProductId", receiptProduct.ProductId);
            cmd.Parameters.AddWithValue("@aQuantity", receiptProduct.Quantity); 
            cmd.Parameters.AddWithValue("@aWareHouseId", receiptProduct.WareHouseId);
            return await cmd.ExecuteNonQueryAsync();
        }

        public async ValueTask<int> UpdateReceiptProduct(ReceiptProduct receiptProduct)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(updateReceiptProductCommand, sqlConnection);
            await sqlConnection.OpenAsync();

            cmd.Parameters.AddWithValue("@aReceptionId", receiptProduct.ReceptionId);
            cmd.Parameters.AddWithValue("@aProductId", receiptProduct.ProductId);
            cmd.Parameters.AddWithValue("@aQuantity", receiptProduct.Quantity);
            cmd.Parameters.AddWithValue("@aWareHouseId", receiptProduct.WareHouseId);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async ValueTask<int> DeleteReceiptProduct(Guid receptionId, Guid productId)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(deleteReceiptProductCommand, sqlConnection);
            await sqlConnection.OpenAsync();

            cmd.Parameters.AddWithValue("@aReceptionId", receptionId);
            cmd.Parameters.AddWithValue("@aProductId", productId);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async ValueTask<ReceiptDetail> GetReceiptInfoById(Guid receiptId, bool includeProducts = true)
        {
            if (receiptId == Guid.Empty)
            {
                throw new ArgumentException("Receipt ID cannot be empty.", nameof(receiptId));
            }

            try
            {
                await using var sqlConnection = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("[reception].[GetById]", sqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@aReceptionId", receiptId);

                await sqlConnection.OpenAsync();

                using var da = new SqlDataAdapter(cmd);
                var ds = new DataSet();
                da.Fill(ds);

                var receiptInfo = GetReceiptFromDataSet(ds, includeProducts);
                if (receiptInfo == null)
                {
                    throw new KeyNotFoundException($"Receipt with ID {receiptId} not found.");
                }

                return receiptInfo;
            }
            catch (SqlException ex)
            {
                throw new Exception($"Database error occurred while retrieving receipt info for ID {receiptId}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving receipt info for ID {receiptId}: {ex.Message}", ex);
            }
        }

        private ReceiptDetail GetReceiptFromDataSet(DataSet ds, bool includeProducts)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return null; // Receipt not found (e.g., return 1001)
            }

            var receiptInfo = GetReceiptDetailFromDataRow(ds.Tables[0].Rows[0]);
            receiptInfo.ReceiptProducts = includeProducts && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0
                ? ds.Tables[1].AsEnumerable().Select(GetReceiptProductInfoFromDataRow).ToList()
                : new List<ReceiptProductInfo>();

            return receiptInfo;
        }

        private ReceiptInfo getReceiptFromDataSet(DataSet ds, bool includeProducts)
        {
            ReceiptInfo receiptInfo = null;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                receiptInfo = GetReceiptInfoFromDataRow(ds.Tables[0].Rows[0]);
                /*  receiptInfo.ReceiptProducts = new List<ReceiptProductInfo>();
                  if (includeProducts)
                  {
                      if (ds.Tables.Count > 1)
                      {
                          foreach (DataRow productRow in ds.Tables[1].Rows)
                          {
                              receiptInfo.ReceiptProducts.Add(GetReceiptProductInfoDataFromDataRow(productRow));
                          }
                      }
                  }*/
            }
            return receiptInfo;
        }

        public async ValueTask<string> ValidateReceipt(Guid receiptId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand("[reception].[Validate]", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@aReceiptId", receiptId);

            var returnValue = new SqlParameter
            {
                ParameterName = "@RETURN_VALUE",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.ReturnValue
            };
            cmd.Parameters.Add(returnValue);

            await cmd.ExecuteNonQueryAsync();

            int result = (int)returnValue.Value;

            switch (result)
            {
                case 2001:
                    return "Cannot validate: receipt deja validee";

                case 2002:
                    return "Cannot validate: rest a livrer <received";
            }
            return "Success";
        }

        public async ValueTask<List<ReceiptInfo>> SelectReceiptsBySupplierId(Guid supplierId)
        {
            var receipts = new List<ReceiptInfo>();
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(selectReceiptsBySupplierIdQuery, sqlConnection);
            cmd.Parameters.AddWithValue("@aSupplierId", supplierId);

            await sqlConnection.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                receipts.Add(GetReceiptDatabyidsupplier(reader));
            }

            return receipts;
        }

        public async ValueTask<bool> ReceiptExistById(Guid id)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("SELECT COUNT(1) FROM [reception].[HEADERS] WHERE Id = @aId", sqlConnection);
            cmd.Parameters.AddWithValue("@aId", id);

            await sqlConnection.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async ValueTask<int> SelectSupplierCount()
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(countofReceptions, sqlConnection);

            await sqlConnection.OpenAsync();
            var count = await cmd.ExecuteScalarAsync();

            return count != null ? Convert.ToInt32(count) : 0;
        }

        public async ValueTask<List<int>> SelectReceptionCountsByStatus()
        {
            var result = new List<int>();

            using var sqlConnection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(countofStatusReception, sqlConnection);

            await sqlConnection.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(reader.GetInt32(0));
            }

            return result;
        }

        public async ValueTask<long> SelectNextReceptionNumber()
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            await sqlConnection.OpenAsync();
            using var cmd = new SqlCommand(selectnewnumberReception, sqlConnection);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        public async ValueTask<long> SetReceptionNumber(Guid purchaseOrderId, long newNumber)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            await sqlConnection.OpenAsync();
            using var cmd = new SqlCommand(setNumberReception, sqlConnection);
            cmd.Parameters.AddWithValue("@aNumber", newNumber.ToString());
            cmd.Parameters.AddWithValue("@aId", purchaseOrderId);

            return await cmd.ExecuteNonQueryAsync();
        }
        public async IAsyncEnumerable<ReceiptDetail> SelectReceptionDetail(Guid receptionId)
        {
            await using var sqlConnection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(getReceptionDetailsProcedure, sqlConnection)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
                Parameters = { new SqlParameter("@aReceptionId", receptionId) }
            };
            await sqlConnection.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var receptionDetail = getReceptionDetailReader(reader);

                await reader.NextResultAsync();
                while (await reader.ReadAsync())
                {
                    receptionDetail.ReceiptProducts.Add(getReceiptProductInfoReader(reader));
                }

                await reader.NextResultAsync();
                if (await reader.ReadAsync())
                {
                    receptionDetail.PurchaseOrder = getPurchaseOrderReader(reader);
                }

                yield return receptionDetail;
            }
        }
    }
}