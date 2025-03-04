using GO.UserControl.Client.Core.Enumerables;

namespace GO.Website.Controllers.Report;

using System;
using System.Threading.Tasks;
using Constants;
using Filters;
using Services;
using Microsoft.AspNetCore.Mvc;

[GOPermission(FunctionKey = FunctionKey.HRAReport, Policy = Policy.Read)]
public class HRAReportController(
    IHraReportService hraReportService,
    IExportExcelService excelService)
    : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Report/HRAReport.cshtml");
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel(int year)
    {
        var data = await hraReportService.GetHraReportAsync(year);

        var bytes = excelService.ExportHRAReport(data);
        var reportName = $"HRAReport_{year}_{DateTime.Today:yyyyMMdd}";
        return File(bytes, "application/xlsx", $"{reportName}.xlsx");
    }
}