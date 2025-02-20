using System;
using GO.Website.Criteria.Manpower;
using GO.Website.Criteria.Report.HRA;
using GO.Website.Models.Manpower.Period;
using GO.Website.Models.Report.HRA;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.Core.Services;

namespace GO.Website.Services;

public interface IHraReportService
{
    Task<HraReportModel> GetHraReportAsync(int year);
}

public class HRAReportService(IBaseService baseService) : IHraReportService
{
    public async Task<HraReportModel> GetHraReportAsync(int year)
    {
        var employeeInfos = await GetEmployeesInfos(year);
        var report = new HraReportModel
        {
            ReportByUnit = CalculateReportByUnit(employeeInfos, year),
            ReportByCompany = CalculateReportByCompany(employeeInfos, year)
        };

        return report;
    }

    public async Task<List<EmployeeInfo>> GetEmployeesInfos(int year)
    {
        var criteria = new GetHRAReportCriteria { Year = year };

        return (await baseService.GetDataAsync<EmployeeInfo>(criteria)).ToList();
    }

    #region CountByMonth
    public List<UnitReport> CalculateReportByUnit(IEnumerable<EmployeeInfo> employeeInfos, int year)
    {
        return employeeInfos
            .GroupBy(x => x.Unit)
            .OrderBy(a => a.Key)
            .Select(groupByUnit => new UnitReport
            {
                Unit = groupByUnit.Key,
                Details = CountByMonth(groupByUnit.Key, groupByUnit.ToList(), year)
            })
            .ToList();
    }

    public IEnumerable<ReportModel> CountByMonth(string unit, List<EmployeeInfo> employeesByUnit, int year)
    {
        for (var i = 1; i <= 12; i++)
        {
            var fromDate = new DateTime(year, i, 1);
            var toDate = new DateTime(year, i, DateTime.DaysInMonth(year, i));
            yield return new ReportModel
            {
                Month = i,
                NewHire = employeesByUnit.Count(x => IsInMonth(x.JoinedDate, fromDate, toDate)),
                ProbationResign = employeesByUnit.Count(x => IsInMonth(x.ResignedDate, fromDate, toDate) && x.ResignedDate <= x.ConfirmedDate),
                ConfirmedResign = employeesByUnit.Count(x => IsInMonth(x.ResignedDate, fromDate, toDate) && x.ResignedDate > x.ConfirmedDate),
                CurrentHeadCount = employeesByUnit.Count(x=> x.JoinedDate <= toDate && (x.ResignedDate is null || x.ResignedDate > toDate))
            };
        }
    }
    #endregion

    #region CountByCompany
    public List<UnitReport> CalculateReportByCompany(IEnumerable<EmployeeInfo> employeeInfos, int year)
    {
        return employeeInfos
            .GroupBy(x => new {x.Country, x.Company})
            .OrderBy(a => a.Key.Country).ThenBy(a=> a.Key.Company)
            .Select(groupByCompany => new UnitReport
            {
                Company = groupByCompany.Key.Company,
                Country = groupByCompany.Key.Country,
                Details = CountByMonth(groupByCompany.Key.Company, groupByCompany.ToList(), year)
            })
            .ToList();
    }
    #endregion

    private static bool IsInMonth(DateTime? date, DateTime fromDate, DateTime toDate) => date >= fromDate && date <= toDate;
}