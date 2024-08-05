namespace GO.Website.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Core.Services;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.Website.Constants;
    using GO.Website.Criteria.Report;
    using GO.Website.Filters;
    using GO.Website.Models.Report.JobLevelSummary;
    using GO.Website.Models.Report.Manpower;
    using GO.Website.Services;
    using Microsoft.AspNetCore.Mvc;

    [GOPermission(FunctionKey = FunctionKey.JobLevelSummaryReport, Policy = Policy.Read)]
    public class JobLevelSummaryReportController : BaseController
    {
        private const byte LengthJobLevelType = 2;

        private readonly Dictionary<string, string> unitTypes = new()
        {
            { "Product Unit", "PU" },
            { "Business Unit", "BU" },
            { "Support Unit", "SU" }
        };

        private readonly Dictionary<string, string[]> shortJobLevelTypes = new()
        {
            { "Supervisor", new[] { "M", "P" } },
            { "Employee", new[] { "E" } }
        };

        private readonly List<string> countryTypes = new() { "Philippines", "Vietnam", "Malaysia", "Taiwan" };

        private readonly IBaseService baseService;
        private readonly IExportExcelService excelService;

        public JobLevelSummaryReportController(
            IBaseService baseService,
            IExportExcelService excelService)
        {
            this.baseService = baseService;
            this.excelService = excelService;
        }

        public IActionResult Index()
        {
            return View("~/Views/Report/JobLevelSummaryReport.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> GetData([FromBody] GetJobLevelSummaryParam param)
        {

            param.JoinedDate = param.JoinedDate.AddMonths(1);
            var criteria = new GetJobLevelSummaryReportCriteria()
            {
                JoinedDate = new DateTime(param.JoinedDate.Year, param.JoinedDate.Month, 1, 0, 0, 0, DateTimeKind.Utc),
            };
            var employeeInfos = await baseService.GetDataAsync<EmployeeInfo>(criteria);

            var listUnits = await GetUnitAsync();
            var listJobLevels = await GetJobLevelAsync();
            var units = GetUnit(listUnits);

            var jobLevels = GetJobLevels(listJobLevels);
            var jobLevelTypes = GetJobLevelTypes(listJobLevels);

            var gridCountryData = GroupGridDataByCountry(jobLevels, units, jobLevelTypes, employeeInfos);
            var gridUnitData = GroupGridDataByUnit(jobLevels, units, jobLevelTypes, employeeInfos);
            var gridCompanyData = GroupGridDataByCompany(jobLevels, jobLevelTypes, employeeInfos);

            return Json(new { employeeInfos, gridCountryData, gridUnitData, gridCompanyData, jobLevelTypes });
        }

        [HttpPost]
        public async Task<IActionResult> ExportExcel([FromBody] GetJobLevelSummaryParam param)
        {
            param.JoinedDate = param.JoinedDate.AddMonths(1);
            var criteria = new GetJobLevelSummaryReportCriteria()
            {
                JoinedDate = new DateTime(param.JoinedDate.Year, param.JoinedDate.Month, 1, 0, 0, 0, DateTimeKind.Utc),
            };
            var employeeInfos = await baseService.GetDataAsync<EmployeeInfo>(criteria);

            var listUnits = await GetUnitAsync();
            var listJobLevels = await GetJobLevelAsync();
            var units = GetUnit(listUnits);

            var jobLevels = GetJobLevels(listJobLevels);
            var jobLevelTypes = GetJobLevelTypes(listJobLevels);

            var gridCountryData = GroupGridDataByCountry(jobLevels, units, jobLevelTypes, employeeInfos);
            var gridUnitData = GroupGridDataByUnit(jobLevels, units, jobLevelTypes, employeeInfos);
            var gridCompanyData = GroupGridDataByCompany(jobLevels, jobLevelTypes, employeeInfos);

            var bytes = excelService.ExportJobLevelSummaryReport(gridUnitData, gridCountryData, gridCompanyData);
            var reportName = $"JobLevelSummaryReportByUnitAndCountry_{DateTime.Today:yyyyMMdd}";
            return File(bytes, "application/xlsx", $"{reportName}.xlsx");
        }

        private async Task<IList<string>> GetUnitAsync()
        {
            var units = await baseService.GetDataAsync<string>(new GetUnitCriteria());
            return units.ToList();
        }

        private async Task<IList<string>> GetJobLevelAsync()
        {
            var jobLevels = await baseService.GetDataAsync<string>(new GetJobLevelCriteria());
            return jobLevels.ToList();
        }

        private Dictionary<string, string[]> GetUnit(IList<string> units)
        {
            var dictionaryUnits = new Dictionary<string, string[]>();
            foreach (var unitType in unitTypes)
            {
                var unitNames = units.Where(x => x[..LengthJobLevelType].Contains(unitType.Value)).ToArray();
                dictionaryUnits.Add(unitType.Key, unitNames);
            }

            var ortherUnit = units.Where(x => !unitTypes.Any(y => x[..LengthJobLevelType].Contains(y.Value))).ToArray();
            dictionaryUnits.Add("Other", ortherUnit);

            return dictionaryUnits;
        }

        private Dictionary<string, string[]> GetJobLevels(IList<string> jobLevels)
        {
            var dictionaryJobLevels = new Dictionary<string, string[]>();
            foreach (var jobLevelType in shortJobLevelTypes)
            {
                var jobLevelNames = new List<string>();
                foreach (var item in jobLevelType.Value)
                {
                    jobLevelNames.AddRange(jobLevels.Where(x => x[..1].Contains(item)).ToList());
                }

                dictionaryJobLevels.Add(jobLevelType.Key, jobLevelNames.ToArray());
            }

            var ortherJobLevels = jobLevels.Where(x => !x[..1].Contains("M") && !x[..1].Contains("P") && !x[..1].Contains("E")).ToList();
            dictionaryJobLevels.Add("Other", ortherJobLevels.ToArray());

            return dictionaryJobLevels;
        }

        private Dictionary<string, string[]> GetJobLevelTypes(IList<string> jobLevels)
        {
            var dictionaryJobLevelTypes = new Dictionary<string, string[]>();
            foreach (var jobLevelType in shortJobLevelTypes)
            {
                var jobLevelNames = new List<string>();
                foreach (var item in jobLevelType.Value)
                {
                    jobLevelNames.AddRange(jobLevels.Where(x => x[..1].Contains(item))
                        .Select(y => y[..LengthJobLevelType]).Distinct().ToArray());
                }

                dictionaryJobLevelTypes.Add(jobLevelType.Key, SortJobLevel(jobLevelNames));
            }

            var ortherJobLevels = new[] { "Other" };
            dictionaryJobLevelTypes.Add("Other", ortherJobLevels.ToArray());

            return dictionaryJobLevelTypes;
        }

        private static string[] SortJobLevel(IList<string> jobLevelNames)
        {
            var sortJobLevels = new List<string>();
            foreach (var range in jobLevelNames.Select(item => int.Parse(item[1..])))
            {
                sortJobLevels.AddRange(jobLevelNames.Where(x => int.Parse(x[1..]).Equals(range) && sortJobLevels.All(y => y != x)).ToList());
            }

            return sortJobLevels.ToArray();
        }

        private GridData GroupGridDataByCountry(
            Dictionary<string, string[]> jobLevels,
            Dictionary<string, string[]> units,
            Dictionary<string, string[]> jobLevelTypes,
            IEnumerable<EmployeeInfo> employeeInfos)
        {
            IList<string> countries = employeeInfos.Where(emp => !string.IsNullOrWhiteSpace(emp.Country))
                .Select(emp => emp.Country).Distinct().OrderBy(item => item).ToList();
            countries.Add("Other");
            var (supervisors, totalSupervisor) = GroupDataByCountry(jobLevels.ElementAt(0).Value, countries, units, employeeInfos);
            var (employees, totalEmployee) = GroupDataByCountry(jobLevels.ElementAt(1).Value, countries, units, employeeInfos);
            var (others, totalOthers) = GroupDataByCountry(jobLevels.ElementAt(2).Value, countries, units, employeeInfos);
            var totalJobTypeSupervisor = GetTotalByJobType(countries, units, jobLevelTypes.ElementAt(0).Value, employeeInfos.ToList());
            var totalJobTypeEmployee = GetTotalByJobType(countries, units, jobLevelTypes.ElementAt(1).Value, employeeInfos.ToList());
            var totalJobTypeOthers = GetTotalByJobType(countries, units, jobLevelTypes.ElementAt(2).Value, employeeInfos.ToList());
            var grandTotal = GetGrandTotal(countries, totalSupervisor, totalEmployee, totalOthers);
            var headers = countries.Select(country => new RowItem
            {
                Name = country,
                Cells = units.Select(item => new GroupCellItem
                {
                    Name = item.Key,
                    Cells = item.Value.Select(unit => new CellItem
                    {
                        Name = unit
                    }).ToList()
                }).ToList()
            }).ToList();

            return new GridData
            {
                Headers = headers,
                Supervisors = supervisors,
                Employees = employees,
                Others = others,
                TotalSupervisor = totalSupervisor,
                TotalEmployee = totalEmployee,
                TotalOthers = totalOthers,
                TotalJobTypeSupervisor = totalJobTypeSupervisor,
                TotalJobTypeEmployee = totalJobTypeEmployee,
                TotalJobTypeOthers = totalJobTypeOthers,
                GrandTotal = grandTotal
            };
        }

        private static RowItem GetGrandTotal(
            IEnumerable<string> countries,
            RowItem totalSupervisor,
            RowItem totalEmployee,
            RowItem totalOther)
        {
            return new RowItem
            {
                Name = "Sub Total",
                Cells = countries.Select(country =>
                {
                    return new GroupCellItem
                    {
                        Name = country,
                        Cells = new List<CellItem>
                        {
                            new CellItem
                            {
                                Total = totalSupervisor.Cells.First(sup => country.Equals(sup.Name)).Cells.Sum(cell => cell.Total)
                                    + totalEmployee.Cells.First(emp => country.Equals(emp.Name)).Cells.Sum(cell => cell.Total)
                                    + totalOther.Cells.First(emp => country.Equals(emp.Name)).Cells.Sum(cell => cell.Total)
                            }
                        }
                    };
                }).ToList()
            };
        }

        private static RowItem GetGrandTotalCompany(
            Dictionary<string, string[]> companies,
            RowItem totalSupervisor,
            RowItem totalEmployee,
            RowItem totalOther)
        {
            return new RowItem
            {
                Name = "Sub Total",
                Cells = companies.Select(country =>
                {
                    return new GroupCellItem
                    {
                        Name = country.Key,
                        Cells = country.Value.Select(company => new CellItem
                        {
                            Total = totalSupervisor.Cells.First(sup => country.Key.Equals(sup.Name)).Cells.Where(x => x.Name.Equals(company)).Sum(cell => cell.Total)
                                    + totalEmployee.Cells.First(emp => country.Key.Equals(emp.Name)).Cells.Where(x => x.Name.Equals(company)).Sum(cell => cell.Total)
                                    + totalOther.Cells.First(emp => country.Key.Equals(emp.Name)).Cells.Where(x => x.Name.Equals(company)).Sum(cell => cell.Total)
                        }).ToList()
                    };
                }).ToList()
            };
        }

        private (IList<RowItem>, RowItem) GroupDataByCountry(
            string[] jobLevels,
            IList<string> countries,
            Dictionary<string, string[]> units,
            IEnumerable<EmployeeInfo> employeeInfos)
        {
            var rows = new List<RowItem>();
            var totalRow = new RowItem
            {
                Name = "Total " + GetJobTypeLevel(jobLevels) + " Level",
                Cells = countries.Select(country => new GroupCellItem
                {
                    Name = country,
                    Cells = units.Select(unit => new CellItem
                    {
                        Name = unit.Key
                    }).ToList()
                }).ToList()
            };

            foreach (var jobLevel in jobLevels)
            {
                var employees = employeeInfos.Where(emp => jobLevel.Equals(emp.JobLevel, StringComparison.CurrentCultureIgnoreCase)).ToList();
                var row = new RowItem
                {
                    Name = jobLevel,
                    Cells = new List<GroupCellItem>()
                };
                foreach (var country in countries)
                {
                    var groupCell = new GroupCellItem
                    {
                        Name = country,
                        Cells = new List<CellItem>()
                    };
                    foreach (var unit in units)
                    {
                        var cell = new CellItem
                        {
                            Name = unit.Key,
                            Total = country == "Other" ? employees.Count(emp => !countryTypes.Any(x => x.Equals(emp.Country, StringComparison.CurrentCultureIgnoreCase)) && unit.Value.Contains(emp.UnitName))
                                : employees.Count(emp => country.Equals(emp.Country, StringComparison.CurrentCultureIgnoreCase) && unit.Value.Contains(emp.UnitName))
                        };
                        groupCell.Cells.Add(cell);

                        totalRow.Cells.First(item => item.Name.Equals(country, StringComparison.CurrentCultureIgnoreCase))
                            .Cells.First(item => item.Name.Equals(unit.Key, StringComparison.CurrentCultureIgnoreCase)).Total += cell.Total;
                    }

                    row.Cells.Add(groupCell);
                }

                rows.Add(row);
            }

            return (rows, totalRow);
        }

        private (IList<RowItem>, RowItem) GroupDataByCompany(
            string[] jobLevels,
            Dictionary<string, string[]> companies,
            IEnumerable<EmployeeInfo> employeeInfos)
        {
            var rows = new List<RowItem>();
            var totalRow = new RowItem
            {
                Name = "Total " + GetJobTypeLevel(jobLevels) + " Level",
                Cells = companies.Select(country => new GroupCellItem
                {
                    Name = country.Key,
                    Cells = country.Value.Select(company => new CellItem { Name = company }).ToList()
                }).ToList()
            };

            foreach (var jobLevel in jobLevels)
            {
                var employees = employeeInfos.Where(emp => jobLevel.Equals(emp.JobLevel, StringComparison.CurrentCultureIgnoreCase)).ToList();
                var row = new RowItem
                {
                    Name = jobLevel,
                    Cells = new List<GroupCellItem>()
                };
                foreach (var country in companies)
                {
                    var groupCell = new GroupCellItem
                    {
                        Name = country.Key,
                        Cells = new List<CellItem>()
                    };
                    foreach (var company in country.Value)
                    {
                        var cell = new CellItem
                        {
                            Name = company,
                            Total = country.Key == "Other" ? employees.Count(emp => !countryTypes.Any(x => x.Equals(emp.Country, StringComparison.CurrentCultureIgnoreCase)) && company.Contains(emp.CompanyName))
                                : employees.Count(emp => country.Key.Equals(emp.Country, StringComparison.CurrentCultureIgnoreCase) && company.Contains(emp.CompanyName))
                        };
                        groupCell.Cells.Add(cell);

                        totalRow.Cells.First(item => item.Name.Equals(country.Key, StringComparison.CurrentCultureIgnoreCase))
                            .Cells.First(item => item.Name.Equals(company, StringComparison.CurrentCultureIgnoreCase)).Total += cell.Total;
                    }

                    row.Cells.Add(groupCell);
                }

                rows.Add(row);
            }

            return (rows, totalRow);
        }

        private GridData GroupGridDataByCompany(
            Dictionary<string, string[]> jobLevels,
            Dictionary<string, string[]> jobLevelTypes,
            IEnumerable<EmployeeInfo> employeeInfos)
        {
            IList<string> countries = employeeInfos.Where(emp => !string.IsNullOrWhiteSpace(emp.Country))
               .Select(emp => emp.Country).Distinct().OrderBy(item => item).ToList();
            countries.Add("Other");
            var companies = new Dictionary<string, string[]>();
            foreach (var item in countries)
            {
                string[] company;
                if (item != "Other")
                {
                    company = employeeInfos.Where(emp => !string.IsNullOrWhiteSpace(emp.CompanyName) && emp.Country == item)
                                .Select(emp => emp.CompanyName).Distinct().OrderBy(item => item).ToArray();
                }
                else
                {
                    company = employeeInfos.Where(emp => !string.IsNullOrWhiteSpace(emp.CompanyName) && !countryTypes.Any(x => x.Contains(emp.Country)))
                                .Select(emp => emp.CompanyName).Distinct().OrderBy(item => item).ToArray();
                }
                companies.Add(item, company);
            }

            var (supervisors, totalSupervisor) = GroupDataByCompany(jobLevels.ElementAt(0).Value, companies, employeeInfos);
            var (employees, totalEmployee) = GroupDataByCompany(jobLevels.ElementAt(1).Value, companies, employeeInfos);
            var (others, totalOthers) = GroupDataByCompany(jobLevels.ElementAt(2).Value, companies, employeeInfos);
            var totalJobTypeSupervisor = GetTotalByCompany(companies, jobLevelTypes.ElementAt(0).Value, employeeInfos.ToList());
            var totalJobTypeEmployee = GetTotalByCompany(companies, jobLevelTypes.ElementAt(1).Value, employeeInfos.ToList());
            var totalJobTypeOthers = GetTotalByCompany(companies, jobLevelTypes.ElementAt(2).Value, employeeInfos.ToList());
            var grandTotal = GetGrandTotalCompany(companies, totalSupervisor, totalEmployee, totalOthers);

            var headers = companies.Select(country => new RowItem
            {
                Name = country.Key,
                Cells = country.Value.Select(company => new GroupCellItem { Name = company }).ToList()
            }).ToList();

            return new GridData
            {
                Headers = headers,
                Supervisors = supervisors,
                Employees = employees,
                Others = others,
                TotalSupervisor = totalSupervisor,
                TotalEmployee = totalEmployee,
                TotalOthers = totalOthers,
                TotalJobTypeSupervisor = totalJobTypeSupervisor,
                TotalJobTypeEmployee = totalJobTypeEmployee,
                TotalJobTypeOthers = totalJobTypeOthers,
                GrandTotal = grandTotal
            };
        }

        private static GridData GroupGridDataByUnit(
            Dictionary<string, string[]> jobLevels,
            Dictionary<string, string[]> units,
            Dictionary<string, string[]> jobLevelTypes,
            IEnumerable<EmployeeInfo> employeeInfos)
        {
            var (supervisors, totalSupervisor) = GroupDataByUnit(jobLevels.ElementAt(0).Value, units, employeeInfos);
            var (employees, totalEmployee) = GroupDataByUnit(jobLevels.ElementAt(1).Value, units, employeeInfos);
            var (others, totalOther) = GroupDataByUnit(jobLevels.ElementAt(2).Value, units, employeeInfos);
            var grandTotal = GetGrandTotalUnit(units, totalSupervisor, totalEmployee, totalOther);
            var totalJobTypeSupervisor = GetTotalByJobType(units, jobLevelTypes.ElementAt(0).Value, employeeInfos);
            var totalJobTypeEmployee = GetTotalByJobType(units, jobLevelTypes.ElementAt(1).Value, employeeInfos);
            var totalJobTypeOther = GetTotalByJobType(units, jobLevelTypes.ElementAt(2).Value, employeeInfos);
            var headers = units.Select(group => new RowItem
            {
                Name = group.Key,
                Cells = group.Value.Select(unit => new GroupCellItem { Name = unit }).ToList()
            }).ToList();

            return new GridData
            {
                Headers = headers,
                Supervisors = supervisors,
                Employees = employees,
                Others = others,
                TotalSupervisor = totalSupervisor,
                TotalEmployee = totalEmployee,
                TotalOthers = totalOther,
                TotalJobTypeSupervisor = totalJobTypeSupervisor,
                TotalJobTypeEmployee = totalJobTypeEmployee,
                TotalJobTypeOthers = totalJobTypeOther,
                GrandTotal = grandTotal
            };
        }

        private static string GetJobTypeLevel(IEnumerable<string> jobLevels)
        {
            var jobTypes = new List<string>();
            foreach (var item in jobLevels)
            {
                if (!jobTypes.Contains(item[..1]))
                {
                    jobTypes.Add(item[..1]);
                }
            }

            return string.Join(", ", jobTypes);
        }

        private static (IList<RowItem>, RowItem) GroupDataByUnit(
            string[] jobLevels,
            Dictionary<string, string[]> units,
            IEnumerable<EmployeeInfo> employeeInfos)
        {
            var rows = new List<RowItem>();
            var totalRow = new RowItem
            {
                Name = "Total " + GetJobTypeLevel(jobLevels) + " Level",
                Cells = units.Select(item => new GroupCellItem
                {
                    Name = item.Key,
                    Cells = item.Value.Select(unit => new CellItem
                    {
                        Name = unit
                    }).ToList()
                }).ToList()
            };

            foreach (var jobLevel in jobLevels)
            {
                var employees = employeeInfos.Where(emp => jobLevel.Equals(emp.JobLevel, StringComparison.CurrentCultureIgnoreCase)).ToList();
                var row = new RowItem
                {
                    Name = jobLevel,
                    Cells = new List<GroupCellItem>()
                };

                foreach (var item in units)
                {
                    var groupCell = new GroupCellItem
                    {
                        Name = item.Key,
                        Cells = new List<CellItem>()
                    };

                    foreach (var unit in item.Value)
                    {
                        var cell = new CellItem
                        {
                            Name = unit,
                            TotalLocal = employees.Count(emp => unit.Equals(emp.UnitName, StringComparison.CurrentCultureIgnoreCase) && emp.IsLocal),
                            TotalExpat = employees.Count(emp => unit.Equals(emp.UnitName, StringComparison.CurrentCultureIgnoreCase) && !emp.IsLocal)
                        };

                        groupCell.Cells.Add(cell);

                        var totalCell = totalRow.Cells.First(itemCell => itemCell.Name.Equals(item.Key, StringComparison.CurrentCultureIgnoreCase))
                            .Cells.First(itemCell => itemCell.Name.Equals(unit, StringComparison.CurrentCultureIgnoreCase));
                        totalCell.TotalLocal += cell.TotalLocal;
                        totalCell.TotalExpat += cell.TotalExpat;
                    }
                    row.Cells.Add(groupCell);
                }
                rows.Add(row);
            }

            return (rows, totalRow);
        }

        private static List<RowItem> GetTotalByJobType(
            Dictionary<string, string[]> units,
            IEnumerable<string> jobLevelTypes,
            IEnumerable<EmployeeInfo> employeeInfos)
        {
            var totalJobType = jobLevelTypes.Select(jobType => new RowItem
            {
                Name = jobType,
                Cells = units.Select(item => new GroupCellItem
                {
                    Name = item.Key,
                    Cells = item.Value.Select(unit => new CellItem { Name = unit }).ToList()
                }).ToList()
            }).ToList();
            foreach (var item in totalJobType)
            {
                var employees = employeeInfos.Where(emp => (item.Name == "Other" && !emp.JobLevel[..1].Contains("M") && !emp.JobLevel[..1].Contains("P") && !emp.JobLevel[..1].Contains("E")) ||
                     (item.Name != "Other" && item.Name.Equals(emp.JobLevel[..LengthJobLevelType], StringComparison.CurrentCultureIgnoreCase))).ToList();
                var row = new RowItem
                {
                    Name = item.Name,
                    Cells = new List<GroupCellItem>()
                };

                foreach (var dataUnit in units)
                {
                    var groupCell = new GroupCellItem
                    {
                        Name = dataUnit.Key,
                        Cells = new List<CellItem>()
                    };

                    foreach (var unit in dataUnit.Value)
                    {
                        var cell = new CellItem
                        {
                            Name = unit,
                            TotalLocal = employees.Count(emp => unit.Equals(emp.UnitName, StringComparison.CurrentCultureIgnoreCase) && emp.IsLocal),
                            TotalExpat = employees.Count(emp => unit.Equals(emp.UnitName, StringComparison.CurrentCultureIgnoreCase) && !emp.IsLocal)
                        };

                        groupCell.Cells.Add(cell);

                        var totalCell = item.Cells.First(itemCell => itemCell.Name.Equals(dataUnit.Key, StringComparison.CurrentCultureIgnoreCase))
                            .Cells.First(itemCell => itemCell.Name.Equals(unit, StringComparison.CurrentCultureIgnoreCase));
                        totalCell.TotalLocal += cell.TotalLocal;
                        totalCell.TotalExpat += cell.TotalExpat;
                    }
                    row.Cells.Add(groupCell);
                }
            }

            return totalJobType;
        }

        private static List<RowItem> GetTotalByJobType(
            IList<string> countries,
            Dictionary<string, string[]> units,
            IEnumerable<string> jobLevelsType,
            IEnumerable<EmployeeInfo> employeeInfos)
        {
            var totalJobType = jobLevelsType.Select(item => new RowItem
            {
                Name = item,
                Cells = countries.Select(country => new GroupCellItem
                {
                    Name = country,
                    Cells = units.Select(unit => new CellItem { Name = unit.Key }).ToList()
                }).ToList()
            }).ToList();

            foreach (var item in totalJobType)
            {
                var employees = employeeInfos.Where(emp => (item.Name == "Other" && !emp.JobLevel[..1].Contains("M") && !emp.JobLevel[..1].Contains("P") && !emp.JobLevel[..1].Contains("E")) ||
                    (item.Name != "Other" && item.Name.Equals(emp.JobLevel[..LengthJobLevelType], StringComparison.CurrentCultureIgnoreCase))).ToList();
                var row = new RowItem
                {
                    Name = item.Name,
                    Cells = new List<GroupCellItem>()
                };

                foreach (var country in countries)
                {
                    var groupCell = new GroupCellItem
                    {
                        Name = country,
                        Cells = new List<CellItem>()
                    };
                    foreach (var unit in units)
                    {
                        var cell = new CellItem
                        {
                            Name = unit.Key,
                            Total = employees
                                .Count(emp => country.Equals(emp.Country, StringComparison.CurrentCultureIgnoreCase) && unit.Value.Contains(emp.UnitName))
                        };
                        groupCell.Cells.Add(cell);

                        item.Cells.First(itemCell => itemCell.Name.Equals(country, StringComparison.CurrentCultureIgnoreCase))
                            .Cells.First(itemCell => itemCell.Name.Equals(unit.Key, StringComparison.CurrentCultureIgnoreCase)).Total += cell.Total;
                    }

                    row.Cells.Add(groupCell);
                }
            }

            return totalJobType;
        }
        private static List<RowItem> GetTotalByCompany(
            Dictionary<string, string[]> companies,
            IEnumerable<string> jobLevelsType,
            IEnumerable<EmployeeInfo> employeeInfos)
        {
            var totalJobType = jobLevelsType.Select(item => new RowItem
            {
                Name = item,
                Cells = companies.Select(country => new GroupCellItem
                {
                    Name = country.Key,
                    Cells = country.Value.Select(company => new CellItem { Name = company }).ToList()
                }).ToList()
            }).ToList();

            foreach (var item in totalJobType)
            {
                var employees = employeeInfos.Where(emp => (item.Name == "Other" && !emp.JobLevel[..1].Contains("M") && !emp.JobLevel[..1].Contains("P") && !emp.JobLevel[..1].Contains("E")) ||
                    (item.Name != "Other" && item.Name.Equals(emp.JobLevel[..LengthJobLevelType], StringComparison.CurrentCultureIgnoreCase))).ToList();
                var row = new RowItem
                {
                    Name = item.Name,
                    Cells = new List<GroupCellItem>()
                };

                foreach (var country in companies)
                {
                    var groupCell = new GroupCellItem
                    {
                        Name = country.Key,
                        Cells = new List<CellItem>()
                    };
                    foreach (var company in country.Value)
                    {
                        var cell = new CellItem
                        {
                            Name = company,
                            Total = employees
                                .Count(emp => country.Key.Equals(emp.Country, StringComparison.CurrentCultureIgnoreCase) && company.Contains(emp.CompanyName))
                        };
                        groupCell.Cells.Add(cell);

                        item.Cells.First(itemCell => itemCell.Name.Equals(country.Key, StringComparison.CurrentCultureIgnoreCase))
                            .Cells.First(itemCell => itemCell.Name.Equals(company, StringComparison.CurrentCultureIgnoreCase)).Total += cell.Total;
                    }

                    row.Cells.Add(groupCell);
                }
            }

            return totalJobType;
        }

        private static RowItem GetGrandTotalUnit(
            Dictionary<string, string[]> units,
            RowItem totalSupervisor,
            RowItem totalEmployee,
            RowItem totalOther)
        {
            return new RowItem
            {
                Name = "Grand Total",
                Cells = units.Select(item => new GroupCellItem
                {
                    Name = item.Key,
                    Cells = item.Value.Select(unit =>
                    {
                        var supervisor = totalSupervisor.Cells.First(itemCell => item.Key.Equals(itemCell.Name)).Cells.First(cell => unit.Equals(cell.Name));
                        var employee = totalEmployee.Cells.First(itemCell => item.Key.Equals(itemCell.Name)).Cells.First(cell => unit.Equals(cell.Name));
                        var other = totalOther.Cells.First(itemCell => item.Key.Equals(itemCell.Name)).Cells.First(cell => unit.Equals(cell.Name));

                        return new CellItem
                        {
                            Name = unit,
                            TotalLocal = supervisor.TotalLocal + employee.TotalLocal + other.TotalLocal,
                            TotalExpat = supervisor.TotalExpat + employee.TotalExpat + other.TotalExpat
                        };
                    }).ToList()
                }).ToList()
            };
        }
    }
}