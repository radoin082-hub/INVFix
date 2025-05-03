using BlazorBootstrap;
using INV.App.Budgets;
using INV.App.Products;
using INV.App.Purchases;
using INV.App.Receipts;
using INV.App.Suppliers;
using INV.App.WareHouses;
using INV.Implementation.Hubs.WareHouses;
using INV.Implementation.Service.Budgets;
using INV.Implementation.Service.Products;
using INV.Implementation.Service.Purchses;
using INV.Implementation.Service.Receipts;
using INV.Implementation.Service.Suppliers;
using INV.Implementation.Service.WareHouses;
using INV.Infrastructure.Storage.Budget;
using INV.Infrastructure.Storage.Products;
using INV.Infrastructure.Storage.Purchases;
using INV.Infrastructure.Storage.Receipts;
using INV.Infrastructure.Storage.SupplierStorages;
using INV.Infrastructure.Storage.WareHouses;
using INV.Web.Components;
using INV.Web.Services.Suppliers;
using Microsoft.AspNetCore.Components.Routing;
using ORG.App.Orgs;
using ORG.Implementation.Service.Orgs;
using ORG.Infrastructure.Storage.Orgs;
using Radzen;
using WareHouseStorage = INV.Infrastructure.Storage.WareHouses.WareHouseStorage;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddScoped<ISupplierStorage, SupplierStorage>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IProductStorage, ProductStorage>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPurchaseOrderStorage, PurchaseOrderStorage>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<IAppSupplierService, AppSupplierService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<IReceiptStorage, ReceiptStorage>();
builder.Services.AddScoped<IWareHouseStorage, WareHouseStorage>();
builder.Services.AddScoped<IWareHouseService, WareHouseService>();
builder.Services.AddScoped<IBudgetStorage, BudgetStorage>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<IOrgService, OrgService>();
builder.Services.AddScoped<IOrgStorage, OrgStorage>();
builder.Services.AddHttpClient();
builder.Services.AddRadzenComponents();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<PreloadService>();
builder.Services.AddScoped<NavigationLock>();
builder.Services.AddLocalization();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddSignalR();
var app = builder.Build();

string[] supportedCultures = ["en", "fr", "ar"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();
app.UseHttpsRedirection();
app.MapControllers();

app.UseAntiforgery();
app.MapStaticAssets();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<WareHouseHub>("/warehouseHub");
    endpoints.MapRazorComponents<App>().AddInteractiveServerRenderMode();
});
app.Run();