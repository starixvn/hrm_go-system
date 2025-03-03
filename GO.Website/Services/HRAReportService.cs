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
            ReportByCompany = CalculateReportByCompany(employeeInfos, year),
            ReportByDetails = CalculateReportByDetails(employeeInfos, year)
        };

        return report;
    }

    public async Task<List<EmployeeInfo>> GetEmployeesInfos(int year)
    {
        var criteria = new GetHRAReportCriteria { Year = year };

        return (await baseService.GetDataAsync<EmployeeInfo>(criteria)).ToList();
    }

    public List<UnitReport> CalculateReportByUnit(IEnumerable<EmployeeInfo> employeeInfos, int year)
    {
        return employeeInfos
            .GroupBy(x => x.Unit)
            .OrderBy(a => a.Key)
            .Select(groupByUnit => new UnitReport
            {
                Unit = groupByUnit.Key,
                Details = CountByMonth(groupByUnit.ToList(), year)
            })
            .ToList();
    }

    public IEnumerable<ReportModel> CountByMonth(List<EmployeeInfo> employeesByUnit, int year)
    {
        for (var i = 1; i <= 12; i++)
        {
            var fromDate = new DateTime(year, i, 1);
            var toDate = new DateTime(year, i, DateTime.DaysInMonth(year, i));
            if (fromDate > DateTime.Now.Date)
            {
                yield return new ReportModel();
            }
            else
            {
                yield return new ReportModel
                {
                    Month = i,
                    NewHire = employeesByUnit.Count(x => IsInMonth(x.JoinedDate, fromDate, toDate)),
                    ProbationResign = employeesByUnit.Count(x => IsInMonth(x.ResignedDate, fromDate, toDate) &&
                                                                 (x.ConfirmedDate is null ||
                                                                  x.ResignedDate <= x.ConfirmedDate)),
                    ConfirmedResign = employeesByUnit.Count(x =>
                        IsInMonth(x.ResignedDate, fromDate, toDate) && x.ResignedDate > x.ConfirmedDate),
                    CurrentHeadCount = employeesByUnit.Count(x =>
                        x.JoinedDate <= toDate && (x.ResignedDate is null || x.ResignedDate > toDate))
                };
            }
        }
    }

    public List<UnitReport> CalculateReportByCompany(IEnumerable<EmployeeInfo> employeeInfos, int year)
    {
        return employeeInfos
            .GroupBy(x => new {x.Country, x.Company})
            .OrderBy(a => a.Key.Country).ThenBy(a=> a.Key.Company)
            .Select(group => new UnitReport
            {
                Company = group.Key.Company,
                Country = group.Key.Country,
                Details = CountByMonth(group.ToList(), year)
            })
            .ToList();
    }

    public List<UnitReport> CalculateReportByDetails(IEnumerable<EmployeeInfo> employeeInfos, int year)
    {
        return employeeInfos
            .GroupBy(x => new { x.Unit, x.LocationCode, x.Company, x.UserType })
            .OrderBy(a => a.Key.Unit)
            .ThenBy(a => a.Key.LocationCode)
            .ThenBy(a => a.Key.Company)
            .ThenBy(a =>a.Key.UserType)
            .Select(group => new UnitReport
            {
                Company = $"{group.Key.UserType} {group.Key.Company}",
                Unit = group.Key.Unit,
                LocationCode = group.Key.LocationCode,
                Details = CountByMonth(group.ToList(), year)
            })
            .ToList();
    }

    private static bool IsInMonth(DateTime? date, DateTime fromDate, DateTime toDate) => date >= fromDate && date <= toDate;
}