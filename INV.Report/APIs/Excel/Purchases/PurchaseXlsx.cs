using System.Globalization;
using INV.App.Purchases;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Humanizer.Localisation;
using Humanizer;

namespace INV.API.APIs.Excel.Purchases;

[Route("purchase")]
[ApiController]
public class PurchaseXlsx(IPurchaseOrderService purchaseOrderService) : Controller
{
    [HttpGet("DownloadExcel/{purchaseId:guid}")]
    public async Task<IActionResult> DownloadExcel(Guid purchaseId)
    {
        var result = await purchaseOrderService.GetPurchaseOrderDetail(purchaseId);
        if (result.IsFailure)
            return BadRequest(result.Error);


        var purchaseDetail = result.Value.FirstOrDefault();
        if (purchaseDetail is null)
            return NotFound("Purchase order not found");


        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("سند الطلب");

        sheet.View.RightToLeft = true;

        sheet.Cells["A1"].Value = "الجمهورية الجزائرية الديمقراطية الشعبية";
        sheet.Cells["A1:G1"].Merge = true;
        sheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

        sheet.Cells["A3"].Value = "سند الطلب";
        sheet.Cells["A3:B3"].Merge = true;
        sheet.Cells["A4"].Value = $"رقم: {purchaseDetail.Number}";
        sheet.Cells["B4"].Value = $"تاريخ: {purchaseDetail.Date}";


        sheet.Cells["A6"].Value = "التعريف بالمصلحة المتعاقدة";
        sheet.Cells["A6:B6"].Merge = true;
        sheet.Cells["A7"].Value = "التسمية: كلية الأداب ولغات";
        sheet.Cells["A8"].Value = "رمز المسير(الأمر بالصرف): 14608";
        sheet.Cells["A9"].Value = "العنوان: ص . ب 123 جامعة محمد خيضر بسكرة";
        sheet.Cells["A10"].Value = "الهاتف و الفاكس: 033 74 00 00";

        sheet.Cells["A12"].Value = "التعريف بالمتعامل اقتصادي";
        sheet.Cells["A12:B12"].Merge = true;
        sheet.Cells["A13"].Value = $"الاسم ولقب: {purchaseDetail.Supplier.CompanyName}";
        sheet.Cells["A14"].Value = $"يتصرف لحساب: {purchaseDetail.Supplier.ManagerName}";
        sheet.Cells["A15"].Value = $"العنوان: {purchaseDetail.Supplier.Address}";
        sheet.Cells["A16"].Value = $"الهاتف الفاكس: {purchaseDetail.Supplier.Phone}";
        sheet.Cells["A17"].Value = $"رقم سجل تجاري: {purchaseDetail.Supplier.RC}";
        sheet.Cells["B17"].Value = $"رقم جبائي: {purchaseDetail.Supplier.ART}";
        sheet.Cells["A18"].Value = $"رقم اعتماد: {purchaseDetail.Supplier.NIF}";
        sheet.Cells["B18"].Value = $"رقم تعريف احصائي: {purchaseDetail.Supplier.NIS}";
        sheet.Cells["A19"].Value = $"كشف حساب بنكي: {purchaseDetail.Supplier.RIB}";
        sheet.Cells["B19"].Value = $"بنك: {purchaseDetail.Supplier.BankAgency}";

        int rows = 24;

        sheet.Cells[$"A{rows}"].Value = "الرقم";
        sheet.Cells[$"B{rows}"].Value = "البيانات";
        sheet.Cells[$"C{rows}"].Value = "وحدة القياس";
        sheet.Cells[$"D{rows}"].Value = "الكمية";
        sheet.Cells[$"E{rows}"].Value = "سعر الوحدة";
        sheet.Cells[$"F{rows}"].Value = "قيمة الضريبة";
        sheet.Cells[$"G{rows}"].Value = "المبلغ";

        rows++;
        foreach (var product in purchaseDetail.Products)
        {
            sheet.Cells[$"A{rows}"].Value = rows - 24;
            sheet.Cells[$"B{rows}"].Value = product.Designation;
            sheet.Cells[$"C{rows}"].Value = product.UnitMeasure;
            sheet.Cells[$"D{rows}"].Value = product.Quantity;
            sheet.Cells[$"E{rows}"].Value = product.UnitPrice;
            sheet.Cells[$"F{rows}"].Value = $"{product.TVA}%";
            sheet.Cells[$"G{rows}"].Value = product.Quantity * product.UnitPrice;
            rows++;
        }

        sheet.Cells[$"F{rows}"].Value = "المبلغ بدون الرسم";
        sheet.Cells[$"G{rows}"].Value = purchaseDetail.TotalHT;
        rows++;
        sheet.Cells[$"F{rows}"].Value = "مبلغ الضريبة";
        sheet.Cells[$"G{rows}"].Value = purchaseDetail.TotalTVA;
        rows++;
        sheet.Cells[$"F{rows}"].Value = "المبلغ الاجمالي";
        sheet.Cells[$"G{rows}"].Value = purchaseDetail.TotalTTC;
        rows++;
        string totalInWords = Convert.ToInt32(purchaseDetail.TotalTTC).ToWords(new CultureInfo("ar"));
        sheet.Cells[$"F{rows}"].Value = "بالعربية";
        sheet.Cells[$"G{rows}"].Value = totalInWords+" دج";


        rows += 2;

        using (var range = sheet.Cells[sheet.Dimension.Address])
        {
            range.Style.Font.Name = "Arial";
            range.Style.Font.Size = 12;
            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            range.Style.ReadingOrder = OfficeOpenXml.Style.ExcelReadingOrder.RightToLeft;
        }

        sheet.Cells["A1:G1"].Style.Font.Bold = true;
        sheet.Cells["A3:B3"].Style.Font.Bold = true;
        sheet.Cells["A6:B6"].Style.Font.Bold = true;
        sheet.Cells["A12:B12"].Style.Font.Bold = true;
        sheet.Cells["A21:D21"].Style.Font.Bold = true;
        sheet.Cells[$"A24:G24"].Style.Font.Bold = true;

        var tables = new[] { "A3:B4", "A6:B10", "A12:B19", "A21:D22", $"A24:G{rows - 2}" };
        foreach (var tableRange in tables)
        {
            using (var range = sheet.Cells[tableRange])
            {
                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }
        }

        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

        var content = package.GetAsByteArray();
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"PurchaseXlsx.xlsx");
    }
}