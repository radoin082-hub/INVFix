using INV.App.Suppliers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Wkhtmltopdf.NetCore;

namespace INV.API.APIs.PDF.Suppliers
{
    [Route("Supplier")]
    [ApiController]
    public class SupplierPdf(ISupplierService supplierService,IGeneratePdf generatePdf) : Controller
    {
        [HttpGet("DowonloadPdf/{supplierId:guid}/Ar")]
        public async Task<IActionResult> DowonloaPdfAr(Guid supplierId)
        {
            var result = await supplierService.GetSupplierById(supplierId);
            var supplierDetail = result.Value;
            
            if (result.IsFailure)
                return BadRequest(result.Error);

            if (supplierDetail is null)
                return NotFound();

            return await generatePdf.GetPdf("Views/Suppliers/SupplierDetailAr.cshtml", supplierDetail);
        }
        [HttpGet("DowonloadPdf/{supplierId:guid}/Fr")]
        public async Task<IActionResult> DowonloaPdfFr(Guid supplierId)
        {
            var result = await supplierService.GetSupplierById(supplierId);
            var supplierDetail = result.Value;
            
            if (result.IsFailure)
                return BadRequest(result.Error);

            if (supplierDetail is null)
                return NotFound();

            return await generatePdf.GetPdf("Views/Suppliers/SupplierDetailFr.cshtml", supplierDetail);
        }
        
    }
}