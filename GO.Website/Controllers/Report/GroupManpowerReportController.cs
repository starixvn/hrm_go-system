namespace GO.Website.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.Website.Constants;
    using GO.Website.Criteria.Report.Manpower;
    using GO.Website.Filters;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Report.Manpower;
    using GO.Website.Services;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    [GOPermission(FunctionKey = FunctionKey.ManpowerForecastReport, Policy = Policy.Read)]
    public class GroupManpowerReportController : Controller
    {
        private readonly IUserControlSession userControlSession;
        private readonly IManpowerForecastService manpowerForecastService;
        private readonly IExportExcelService excelService;

        public GroupManpowerReportController(
            IUserControlSession userControlSession,
            IManpowerForecastService manpowerForecastService,
            IExportExcelService excelService)
        {
            this.userControlSession = userControlSession;
            this.manpowerForecastService = manpowerForecastService;
            this.excelService = excelService;
        }

        public IActionResult Index()
        {
            return View("~/Views/Report/GroupManpowerReport.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> GetRequests([FromBody] GetManpowerForecastReportParam param)
        {
            var criteria = new GetManpowerForecastReportCriteria
            {
                UserId = userControlSession.UserId,
                SiteId = userControlSession.SiteId,
                PeriodId = param.PeriodId,
                UnitType = param.UnitType
            };
            var requests = await manpowerForecastService.GetManpowerForecastReportAsync(criteria);

            return Json(requests);
        }

        [HttpPost]
        public async Task<IActionResult> ExportExcel([FromBody] GetGroupManpowerReportParam param)
        {
            var criteria = new GetGroupManpowerReportCriteria
            {
                UserId = userControlSession.UserId,
                SiteId = userControlSession.SiteId,
                Month = param.Month,
                Year = param.Year
            };
            //var requests = await manpowerForecastService.GetManpowerForecastReportAsync(criteria);

            var bytes = excelService.ExportGroupManpowerReport(new List<AnnualManpowerForecastRequest>(), param.Year, "DefaultPeriodName");
            var reportName = $"GroupManpower_{DateTime.Today:yyyyMMdd}";
            return File(bytes, "application/xlsx", $"{reportName}.xlsx");
        }
    }
}