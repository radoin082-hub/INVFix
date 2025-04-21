using INV.API.APIs.Models;
using INV.App.Purchases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Wkhtmltopdf.NetCore;

namespace INV.Report.APIs.PDF.Purchases;

[Route("purchase")]
[ApiController]
public class PurchasePdf(
    IGeneratePdf generatePdf,
    IPurchaseOrderService purchaseOrderService,
    IOptions<ApiSettings> apiSettings)
    : Controller
{
    private readonly string apiKey = apiSettings.Value.ApiKey;
    private bool isAuthorized()
    {
        if (!Request.Headers.TryGetValue("API-KEY", out var providedKey))
            return false;

        return apiKey == providedKey;
    }

    [HttpGet("DownloadPdf/{purchaseId:guid}/Ar")]
    public async Task<IActionResult> DownloadPdfAr(Guid purchaseId)
    {
        /*
        if (!isAuthorized())
            return Unauthorized("Unauthorized access");
            */

        var result = await purchaseOrderService.GetPurchaseOrderDetail(purchaseId);
        var purchaseDetail = result.Value.FirstOrDefault();

        if (result.IsFailure)
            return BadRequest("Error while fetching purchase order");

        if (purchaseDetail is null)
            return NotFound("Purchase Not Found");

        return await generatePdf.GetPdf("Views/Purchases/PurchaseOrderAr.cshtml", purchaseDetail);
    }

    [HttpGet("DownloadPdf/{purchaseId:guid}/Fr")]
    public async Task<IActionResult> DownloadPdfFr(Guid purchaseId)
    {
        /*if (!isAuthorized())
            return Unauthorized("Unauthorized access");*/

        var result = await purchaseOrderService.GetPurchaseOrderDetail(purchaseId);
        var purchaseDetail = result.Value.FirstOrDefault();

        if (result.IsFailure)
            return BadRequest("Error while fetching purchase order");

        if (purchaseDetail is null)
            return NotFound("Purchase Not Found");

        return await generatePdf.GetPdf("Views/Purchases/PurchaseOrderFr.cshtml", purchaseDetail);
    }
}