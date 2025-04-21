using INV.App.Receipts;
using Microsoft.AspNetCore.Mvc;
using Wkhtmltopdf.NetCore;
using Wkhtmltopdf.NetCore.Options;


namespace INV.API.APIs.PDF.Receipts;

[Route("receipt")]
[ApiController]
public class ReceptionPdf(IGeneratePdf generatePdf, IReceiptService receiptService) : Controller
{
    [HttpGet("DownloadPdf/{receiptId:guid}/Fr")]
    public async Task<IActionResult> DownloadPdfFr(Guid receiptId)
    {
        var result = await receiptService.GetReceiptDetail(receiptId);
        var receiptDetail = result.Value.FirstOrDefault();
        if (result.IsFailure)
            return BadRequest(result.Error);

        if (receiptDetail is null)
            return NotFound();

        return await generatePdf.GetPdf("Views/Receptions/ReceptionDetailFr.cshtml", receiptDetail);
    }

    [HttpGet("DownloadPdf/{receiptId:guid}/Ar")]
    public async Task<IActionResult> DownloadPdfAr(Guid receiptId)
    {
        var result = await receiptService.GetReceiptDetail(receiptId);
        var receiptDetail = result.Value.FirstOrDefault();

        if (result.IsFailure)
            return BadRequest(result.Error);

        if (receiptDetail is null)
            return NotFound();

        return await generatePdf.GetPdf("Views/Receptions/ReceptionDetailAr.cshtml", receiptDetail);
    }
}