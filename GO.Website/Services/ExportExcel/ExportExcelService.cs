namespace GO.Website.Services
{
    using System.Collections.Generic;
    using System.IO;
    using ExportExcel;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Report.HRA;
    using GO.Website.Models.Report.Manpower;
    using Microsoft.AspNetCore.Hosting;
    using EmployeeInfo = Models.Manpower.EmployeeInfo;

    public class ExportExcelService(IWebHostEnvironment hostingEnvironment) : IExportExcelService
    {
        public byte[] ExportAnnualManpowerForecastRequestForm(AnnualManpowerForecastRequest request)
        {
            const string excelTemplatePath = "ExcelTemplates/AnnualManpowerForecastRequestForm.xlsx";
            var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, excelTemplatePath);
            var excel = new AnnualManpowerForecastRequestFormExcel(templatePath, request);
            return excel.Export();
        }

        public byte[] ExportJobLevelSummaryReport(GridData gridUnitData, GridData gridCountryData)
        {
            const string excelTemplatePath = "ExcelTemplates/JobLevelSummaryReportByUnitAndCountry.xlsx";
            var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, excelTemplatePath);
            var excel = new JobLevelSummaryReportExcel(templatePath, gridUnitData, gridCountryData);
            return excel.Export();
        }

        public byte[] ExportManpowerForecastReport(IEnumerable<AnnualManpowerForecastRequest> requests, int year, string periodName)
        {
            const string excelTemplatePath = "ExcelTemplates/ManpowerForecast.xlsx";
            var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, excelTemplatePath);
            var excel = new ManpowerForecastReportExcel(templatePath, requests, year, periodName);
            return excel.Export();
        }

        public byte[] ExportEmployeeReport(IEnumerable<EmployeeInfo> employeeInfos)
        {
            const string excelTemplatePath = "ExcelTemplates/EmployeeInfo.xlsx";
            var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, excelTemplatePath);
            var excel = new EmployeeInFoReportExcel(templatePath, employeeInfos);
            return excel.Export();
        }

        public byte[] ExportHRAReport(HraReportModel data)
        {
            const string excelTemplatePath = "ExcelTemplates/HRAReportTemplate.xlsx";
            var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, excelTemplatePath);
            var excel = new HraReportExcel(templatePath, data);
            return excel.Export();
        }
    }
}