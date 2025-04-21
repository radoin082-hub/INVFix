using System.Data;
using System.Runtime.Intrinsics.Arm;
using INV.App.Purchases;
using INV.Domain.Entities.Budget;
using INV.Domain.Entities.Products;
using INV.Domain.Entities.Purchases;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PuppeteerSharp.Cdp;

namespace INV.Infrastructure.Storage.Purchases
{
    public partial class PurchaseOrderStorage : IPurchaseOrderStorage
    {
        private readonly string _connectionString;

        public PurchaseOrderStorage(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("INV");
        }

        private const string selectAllPurchaseOrderByDateQuery =
            " SELECT * FROM [purchase].[ORDERS] WHERE CAST(Date AS DATE) = @aSelectedDate";

        private const string selectPurchaseOrdersInfoQuery = "select * From Purchase.GetList()";

        private const string selectAllPurchaseOrderByIdSupplierQuery =
            "SELECT * FROM purchase.GetListBySupplier(@aSupplierId)";

        private const string selectPurchaseProductsQuery = @" SELECT * FROM [purchase].[PRODUCTS]";

        private const string selectPurchceOrderByIdQuery = @"
    SELECT O.*, S.CompanyName AS SupplierName
    FROM [INV].[purchase].[ORDERS] O
    JOIN [INV].[dbo].[SUPPLIERS] S ON O.SupplierId = S.Id
    WHERE O.Id = @aId;";

        private const string insertOrderDetailCommand = @"
            INSERT INTO [purchase].[PRODUCTS] (PurchaseId, ProductId, Quantity, UnitPrice)
            VALUES (@aPurchaseId, @aProductId, @aQuantity, @aUnitPrice)";

        private const string insertPurchaseOrderCommand = @"
            INSERT INTO [purchase].[ORDERS] (Id, Number, SupplierId, Date, BudgetChapter, BudgetArticle,
                    BudgetType,ServiceType, TotalHT, TotalVA, TotalTC, CompletionDelay)
            VALUES (@aId, @aNumber, @aSupplierId, @aDate,@aBudgetChapter, @aBudgetArticle, @aBudgetType,
                    @aServiceType, @aTotalHT, @aTotalTVA, @aTotalTTC, @aCompletionDelay)";

        private const string DecisionCFPurchaseCommand =
            @" UPDATE purchase.ORDERS SET VisaNumber=@aVisaNumber , VisaDate=@aVisaDate ,Status=@aStatus , Observation=@aMotif Where Id=@aId";

        private const string SelectPurchasesForReceiptCreationCommand = "reception.SelectPurchasesForReceiptCreation";

        private const string selectProductsByPurchaseIdQuery = @"
            SELECT
     p.PurchaseId,
     p.ProductId,
     dp.Designation,
     p.Quantity AS Quantity,
     p.UnitPrice AS UnitPrice,
     dp.TVA,
     w.Name,
     p.Received
 FROM [INV].[purchase].[PRODUCTS] p
 INNER JOIN [INV].[dbo].[PRODUCTS] dp ON p.ProductId = dp.Id
  INNER JOIN [INV].[dbo].[WareHouse] w ON w.Id = dp.DefaultWareHouseId

            WHERE p.PurchaseId = @aPurchaseId;";

        private const string updatePurchaseOrderCommand = @"
            UPDATE [purchase].[ORDERS]
            SET Number=@aNumber, SupplierId=@aSupplierId, Date=@aDate, BudgetArticle=@aBudgetArticle,
                BudgetType=@aBudgetType, ServiceType=@aServiceType, TotalHT=@aTotalHT,
                TotalVA=@aTotalTVA, TotalTC=@aTotalTTC, CompletionDelay=@aCompletionDelay,BudgetChapter=@aBudgetChapter
            WHERE Id=@aId";

        private const string deletePurchaseProductCommand =
            "DELETE FROM [purchase].[PRODUCTS] WHERE PurchaseId=@aPurchaseId AND ProductId=@aProductId";

        private const string selectPurchaseStatusQuery = "SELECT Status FROM [purchase].[ORDERS] WHERE Id=@aId";

        private const string UpdatePurchaseProductQuery = @"UPDATE [INV].[purchase].[PRODUCTS]
        SET Quantity = @aQuantity, UnitPrice = @aUnitPrice
        WHERE PurchaseId = @aPurchaseOrderId
        AND ProductId = @aProductId;";

        private const string insertProductPurchaseCommand = @"
            INSERT INTO [purchase].[PRODUCTS] ( PurchaseId, ProductId,Quantity,UnitPrice)
            VALUES (@aPurchaseId, @aProductId,@aQuantity, @aUnitPrice)";

        private const string countofPurchases = @"SELECT COUNT(*)  FROM [purchase].[ORDERS];";

        private const string countofStatusPurchases = @" SELECT COUNT(o.Status) AS Count
                                                        FROM (
                                                            SELECT 0 AS Status
                                                            UNION ALL SELECT -1
                                                            UNION ALL SELECT 1
                                                            UNION ALL SELECT 2
                                                        ) AS s
                                                        LEFT JOIN [purchase].[ORDERS] AS o
                                                            ON o.Status = s.Status
                                                            AND YEAR(o.Date) = YEAR(GETDATE())
                                                        GROUP BY s.Status
                                                        ORDER BY s.Status;";

        private const string selectnewnumberPurchaseOrder = @"SELECT MAX(CAST([Number] AS BIGINT)) + 1 AS NextNumber
FROM [INV].[purchase].[ORDERS]
WHERE ISNUMERIC([Number]) = 1;";

        private const string setNumberPurchaseOrder = @" UPDATE [INV].[purchase].[ORDERS]
SET [Number] = @aNumber
WHERE [Id] = @aId;";

        private const string selectPuchaseDetailQuery = "dbo.GetPurchaseDetailsById";

        public async IAsyncEnumerable<PurchaseOrderInfo> SelectPurchaseOrderInfo()
        {
            await using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(selectPurchaseOrdersInfoQuery, sqlConnection);
            await sqlConnection.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return getPurchaseOrdersInfoData(reader);
            }
        }

        public async Task<int> InsertPurchaseOrder(PurchaseOrder purchaseOrder)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(insertPurchaseOrderCommand, sqlConnection);
            await sqlConnection.OpenAsync();

            cmd.Parameters.AddWithValue("@aId", purchaseOrder.Id);
            cmd.Parameters.AddWithValue("@aNumber", "0");
            cmd.Parameters.AddWithValue("@aSupplierId", purchaseOrder.SupplierId);
            cmd.Parameters.AddWithValue("@aDate", purchaseOrder.Date);
            cmd.Parameters.AddWithValue("@aBudgetChapter", purchaseOrder.BudgetChapter);
            cmd.Parameters.AddWithValue("@aBudgetArticle", purchaseOrder.BudgetArticle);
            cmd.Parameters.AddWithValue("@aBudgetType", purchaseOrder.BudgetType);
            cmd.Parameters.AddWithValue("@aServiceType", purchaseOrder.ServiceType);
            cmd.Parameters.AddWithValue("@aTotalHT", purchaseOrder.TotalHT);
            cmd.Parameters.AddWithValue("@aTotalTVA", purchaseOrder.TotalTVA);
            cmd.Parameters.AddWithValue("@aTotalTTC", purchaseOrder.TotalTTC);
            cmd.Parameters.AddWithValue("@aCompletionDelay", purchaseOrder.CompletionDelay);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<PurchaseOrder>> SelectPurchaseOrdersByDate(DateOnly selectedDate)
        {
            var purchaseOrders = new List<PurchaseOrder>();

            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(selectAllPurchaseOrderByDateQuery, sqlConnection);

            cmd.Parameters.AddWithValue("@aSelectedDate", selectedDate);

            await sqlConnection.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var purchaseOrder = getPurchaseOrdersData(reader);
                purchaseOrders.Add(purchaseOrder);
            }

            return purchaseOrders;
        }

        public async Task<PurchaseOrder?> SelectPurchaseOrdersByID(Guid id)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            await sqlConnection.OpenAsync();

            using var cmd = new SqlCommand(selectPurchceOrderByIdQuery, sqlConnection);
            cmd.Parameters.AddWithValue("@aId", id);

            using var reader = await cmd.ExecuteReaderAsync();

            return await reader.ReadAsync() ? getPurchaseOrdersData(reader) : null;
        }

        public async ValueTask InsertProductPurchase(PurchaseProduct purchaseProduct)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(insertProductPurchaseCommand, sqlConnection);
            await sqlConnection.OpenAsync();

            cmd.Parameters.AddWithValue("@aPurchaseId", purchaseProduct.PurchaseOrderId);
            cmd.Parameters.AddWithValue("@aProductId", purchaseProduct.ProductId);
            cmd.Parameters.AddWithValue("@aQuantity", purchaseProduct.Quantity);
            cmd.Parameters.AddWithValue("@aUnitPrice", purchaseProduct.UnitPrice);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<PurchaseProduct>> SelectAllPurchaseProduct()
        {
            var orderDetails = new List<PurchaseProduct>();

            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(selectPurchaseProductsQuery, sqlConnection);
            await sqlConnection.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                orderDetails.Add(getPurchaseProductsData(reader));
            }

            return orderDetails;
        }

        public async Task<List<PurchaseOrderInfo>> SelectPurchaseOrdersByIdSupplier(Guid supplierId)
        {
            var purchaseOrders = new List<PurchaseOrderInfo>();

            await using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(selectAllPurchaseOrderByIdSupplierQuery, sqlConnection);

            cmd.Parameters.AddWithValue("@aSupplierId", supplierId);

            await sqlConnection.OpenAsync();
            var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var purchaseOrder = getPurchaseOrdersInfoData(reader);
                purchaseOrders.Add(purchaseOrder);
            }

            return purchaseOrders;
        }

        public async ValueTask<List<PurchaseOrderInfo>> SelectPurchasesForReceiptCreation()
        {
            var purchaseOrdersInfo = new List<PurchaseOrderInfo>();
            var purchaseProducts = new List<PurchaseProduct>();

            using var sqlConnection = new SqlConnection(_connectionString);
            await sqlConnection.OpenAsync();

            using var cmd = new SqlCommand(SelectPurchasesForReceiptCreationCommand, sqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                purchaseOrdersInfo.Add(getPurchaseForcreationData(reader));
            }

            return (purchaseOrdersInfo);
        }

        //DR
        public async ValueTask<List<PurchaseProductInfo>> SelectProductsByPurchaseId(Guid purchaseId)
        {
            var products = new List<PurchaseProductInfo>();

            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(selectProductsByPurchaseIdQuery, sqlConnection);
            cmd.Parameters.AddWithValue("@aPurchaseId", purchaseId);

            await sqlConnection.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new PurchaseProductInfo
                {
                    PurchaseId = (Guid)reader["PurchaseId"],
                    ProductId = (Guid)reader["ProductId"],
                    Designation = (string)reader["Designation"],
                    TVA = (int)reader["TVA"],
                    Quantity = (int)reader["Quantity"],
                    UnitPrice = (decimal)reader["UnitPrice"],
                    Received = (int)reader["Received"],
                    WareHouse = (string)reader["Name"]
                });
            }

            return products;
        }

        //new

        public async ValueTask<int> DeletePurchaseProduct(Guid productId, Guid purchaseId)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(deletePurchaseProductCommand, sqlConnection);
            cmd.Parameters.AddWithValue("@aPurchaseId", purchaseId);
            cmd.Parameters.AddWithValue("@aProductId", productId);
            await sqlConnection.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async ValueTask<int> DeleteAllPurchaseProduct(Guid purchaseOrderId)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(deletePurchaseProductCommand, sqlConnection);
            cmd.Parameters.AddWithValue("@aPurchaseId", purchaseOrderId);

            await sqlConnection.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async ValueTask<int> SetPurchaseProduct(PurchaseProduct purchaseProduct)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(UpdatePurchaseProductQuery, sqlConnection);

            cmd.Parameters.AddWithValue("@aPurchaseOrderId", purchaseProduct.PurchaseOrderId);
            cmd.Parameters.AddWithValue("@aProductId", purchaseProduct.ProductId);
            cmd.Parameters.AddWithValue("@aQuantity", purchaseProduct.Quantity);
            cmd.Parameters.AddWithValue("@aUnitPrice", purchaseProduct.UnitPrice);
            await sqlConnection.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async ValueTask<int> SetPurchaseOrder(PurchaseOrder purchaseOrder)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(updatePurchaseOrderCommand, sqlConnection);
            cmd.Parameters.AddWithValue("@aId", purchaseOrder.Id);
            cmd.Parameters.AddWithValue("@aNumber", 1);
            cmd.Parameters.AddWithValue("@aSupplierId", purchaseOrder.SupplierId);
            cmd.Parameters.AddWithValue("@aDate", purchaseOrder.Date);
            cmd.Parameters.AddWithValue("@aBudgetArticle", purchaseOrder.BudgetArticle);
            cmd.Parameters.AddWithValue("@aBudgetType", purchaseOrder.BudgetType);
            cmd.Parameters.AddWithValue("@aBudgetChapter", purchaseOrder.BudgetChapter);
            cmd.Parameters.AddWithValue("@aServiceType", purchaseOrder.ServiceType);
            cmd.Parameters.AddWithValue("@aTotalHT", 1);
            cmd.Parameters.AddWithValue("@aTotalTVA", 1);
            cmd.Parameters.AddWithValue("@aTotalTTC", 1);
            cmd.Parameters.AddWithValue("@aCompletionDelay", purchaseOrder.CompletionDelay);

            await sqlConnection.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async ValueTask<PurchaseStatus> selectPurchaseStatus(Guid id)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(selectPurchaseStatusQuery, sqlConnection);
            cmd.Parameters.AddWithValue("@aId", id);
            await sqlConnection.OpenAsync();
            var status = (PurchaseStatus)await cmd.ExecuteScalarAsync();
            return status;
        }

        public Task<int> InsertPurchaseProduct(PurchaseProduct orderDetail)
        {
            throw new NotImplementedException();
        }

        public async ValueTask DecinsonCF(Guid purchaseId, PurchaseStatus status, DateOnly? date, string? visaNumber, string? motif)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(DecisionCFPurchaseCommand, sqlConnection);
            await sqlConnection.OpenAsync();

            cmd.Parameters.AddWithValue("@aId", purchaseId);
            cmd.Parameters.AddWithValue("@aVisaNumber", (object?)visaNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@aVisaDate", (object?)date ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@aStatus", (object?)status ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@aMotif", (object?)motif ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        public async ValueTask<int> SelectPurchaseCount()
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(countofPurchases, sqlConnection);

            await sqlConnection.OpenAsync();
            var count = await cmd.ExecuteScalarAsync();

            return count != null ? Convert.ToInt32(count) : 0;
        }

        public async ValueTask<List<int>> SelectPurchaseCountsByStatus()
        {
            var result = new List<int>();

            using var sqlConnection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(countofStatusPurchases, sqlConnection);

            await sqlConnection.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(reader.GetInt32(0));
            }

            return result;
        }

        public async ValueTask<long> SelectNextPurchaseOrderNumberAsync()
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            await sqlConnection.OpenAsync();
            using var cmd = new SqlCommand(selectnewnumberPurchaseOrder, sqlConnection);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        public async ValueTask<long> SetPurchaseOrderNumberAsync(Guid purchaseOrderId, long newNumber)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            await sqlConnection.OpenAsync();
            using var cmd = new SqlCommand(setNumberPurchaseOrder, sqlConnection);
            cmd.Parameters.AddWithValue("@aNumber", newNumber.ToString());
            cmd.Parameters.AddWithValue("@aId", purchaseOrderId);

            return await cmd.ExecuteNonQueryAsync();
        }
        public async IAsyncEnumerable<PurchaseDetail> SelectPurchaseDetail(Guid purchaseId)
        {
            await using var sqlConnection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(selectPuchaseDetailQuery, sqlConnection)
            {
                CommandType = CommandType.StoredProcedure,
                Parameters = { new SqlParameter("@aPurchaseId", purchaseId) }
            };
            await sqlConnection.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var purchaseDetail = getPurchaseDetailData(reader);
      
                await reader.NextResultAsync();
                while (await reader.ReadAsync())
                {
                    purchaseDetail.Products.Add(getProductsData(reader));
                    
                }
        
                yield return purchaseDetail;
            }
        }
    }
}