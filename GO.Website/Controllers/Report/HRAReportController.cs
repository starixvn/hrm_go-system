namespace GO.Website.Controllers.Report;

using System;
using System.Threading.Tasks;
using GO.Website.Constants;
using GO.Website.Criteria.Report.HRA;
using GO.Website.Criteria.Report.Manpower;
using GO.Website.Filters;
using GO.Website.Models.Report.Manpower;
using GO.Website.Services;
using Microsoft.AspNetCore.Mvc;

//[GOPermission(FunctionKey = FunctionKey.HRAReport, Policy = Policy.Read)]
public class HRAReportController : Controller
{
    private readonly IHraReportService hraReportService;
    private readonly IExportExcelService excelService;

    public HRAReportController(
        IHraReportService hraReportService,
        IExportExcelService excelService)
    {
        this.hraReportService = hraReportService;
        this.excelService = excelService;
    }

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