using INV.App.Purchases;
using INV.App.Receipts;
using INV.App.Suppliers;
using INV.Implementation.Service.Purchses;
using INV.Implementation.Service.Receipts;
using INV.Implementation.Service.Suppliers;
using INV.Infrastructure.Storage.Products;
using INV.Infrastructure.Storage.Purchases;
using INV.Infrastructure.Storage.Receipts;
using INV.Infrastructure.Storage.SupplierStorages;
using OfficeOpenXml;
using Wkhtmltopdf.NetCore;

var builder = WebApplication.CreateBuilder(args);
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddWkhtmltopdf("wkhtmltopdf");
builder.Services.AddScoped<IPurchaseOrderStorage, PurchaseOrderStorage>();
builder.Services.AddScoped<IProductStorage, ProductStorage>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<IReceiptStorage, ReceiptStorage>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<ISupplierService,SupplierService>();
builder.Services.AddScoped<ISupplierStorage, SupplierStorage>();
/*
builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));
*/
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });


app.Run();