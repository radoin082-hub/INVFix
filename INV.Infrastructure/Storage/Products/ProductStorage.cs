using INV.App.Products;
using INV.App.Receipts;
using System.Data;
using INV.Domain.Entities.Products;
using INV.Domain.Entities.Receipts;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace INV.Infrastructure.Storage.Products
{
    public partial class ProductStorage : IProductStorage
    {
        private readonly string _connectionString;

        public ProductStorage(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("INV");
        }

        private const string insertProductCommand = @"
            INSERT INTO [dbo].[PRODUCTS] ( Id, Designation,UnitMeasure,Quantity, TVA,DefaultWareHouseId)
            VALUES (@aId, @aDesignation,@aUnitMeasure, @aQuantity, @aTVA, @aDefaultWareHouseId)";

        private const string updateProductCommand = @"
            UPDATE [dbo].[PRODUCTS] SET Designation = @aDesignation, UnitMeasure = @aUnitMeasure,Quantity = @aQuantity,
           TVA = @aTVA  WHERE Id = @aId";

        private const string deleteProductCommand = @"
            Delete from [dbo].[PRODUCTS] where Id=@aId";

        private const string selectProductsQuery = @"
            SELECT * FROM [dbo].[PRODUCTS] ";

        private const string selectProductCountByIdQuery = @"
            SELECT count(*) FROM Products WHERE Designation = @aDesignation";
        

       private const string selectPurchaseCountByProductIdQuery = 
           "SELECT COUNT(*) FROM [purchase].[PRODUCTS] WHERE ProductId = @aProductId";   
  

        public async Task<int> InsertProduct(Product product)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(insertProductCommand, sqlConnection);
            await sqlConnection.OpenAsync();

            cmd.Parameters.AddWithValue("@aId", product.Id);
            cmd.Parameters.AddWithValue("@aDesignation", product.Designation);
            cmd.Parameters.AddWithValue("@aUnitMeasure", product.UnitMeasure);
            cmd.Parameters.AddWithValue("@aQuantity", product.Quantity);
            cmd.Parameters.AddWithValue("@aUnitPrice", product.UnitPrice);
            cmd.Parameters.AddWithValue("@aTVA", product.TVA);
            cmd.Parameters.AddWithValue("@aDefaultWareHouseId", product.DefaultWareHouseId);
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> UpdateProduct(Product product)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(updateProductCommand, sqlConnection);
            await sqlConnection.OpenAsync();

            cmd.Parameters.AddWithValue("@aId", product.Id);
            cmd.Parameters.AddWithValue("@aDesignation", product.Designation);
            cmd.Parameters.AddWithValue("@aUnitMeasure", product.UnitMeasure);
            cmd.Parameters.AddWithValue("@aQuantity", product.Quantity);
            cmd.Parameters.AddWithValue("@aTVA", product.TVA);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteProduct(Guid id)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(deleteProductCommand, sqlConnection);
            await sqlConnection.OpenAsync();
            cmd.Parameters.AddWithValue("@aId", id);
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<ProductInfo>> SelectProducts()
        {
            var products = new List<ProductInfo>();

            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(selectProductsQuery, sqlConnection);
            await sqlConnection.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var product = MapToProductInfo(reader); // Utilisation du mapper
                products.Add(product);
            }

            return products;
        }

        public async Task<bool> ProductExistsByaDesignation(string designation)
        {
            using var connection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(selectProductCountByIdQuery, connection);
            cmd.Parameters.AddWithValue("@aDesignation", designation);
            await connection.OpenAsync();

            int count = (int)(await cmd.ExecuteScalarAsync() ?? 0);
            return count > 0;
        }

        public async ValueTask<ProductDetail> GetProductById(Guid productId)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            await sqlConnection.OpenAsync();

            using var cmd = new SqlCommand("[dbo].[GetProductById]", sqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@ProductId", productId);

            var dataSet = new DataSet();
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dataSet);

            return MapToProductDetail(dataSet); // Utilisation du mapper
        }
        public async ValueTask<bool> SelectPurchaseCountByProductId(Guid productId)
        {
            using var sqlConnection = new SqlConnection(_connectionString);
            var cmd = new SqlCommand(selectPurchaseCountByProductIdQuery, sqlConnection);
            cmd.Parameters.AddWithValue("@aProductId", productId);
            await sqlConnection.OpenAsync();
            int count = (int)(await cmd.ExecuteScalarAsync() ?? 0);
            return count > 0;
        }
    }
}