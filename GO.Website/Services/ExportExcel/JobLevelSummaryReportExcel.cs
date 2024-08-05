namespace GO.Website.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using GO.Website.Models.Report.Manpower;
    using OfficeOpenXml;

    public class JobLevelSummaryReportExcel : ExcelBase
    {
        private readonly GridData gridUnitData;
        private readonly GridData gridCountryData;
        private readonly GridData gridCompanyData;
        private int _startCountryColIndex;
        private int _startCompanyColIndex;
        private const byte RowHeaderIndex = 2;
        private const byte RowBodyIndex = 5;
        private const byte TotalRowIndex = 6;
        private const byte GrandTotalRowIndex = 7;
        private const byte Two = 2;
        private const byte Three = 3;
        private const byte Four = 4;
        private const byte CountryJobLevelColIndex = 14;
        private const byte CountryColIndex = 16;

        public JobLevelSummaryReportExcel(
               string templatePath,
               GridData gridUnitData,
               GridData gridCountryData,
               GridData gridCompanyData) : base(templatePath, 0, "Summary")
        {
            this.gridUnitData = gridUnitData;
            this.gridCountryData = gridCountryData;
            this.gridCompanyData = gridCompanyData;
        }

        protected internal override void RenderTitle(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[1, 1].Copy(worksheet.Cells[1, 1]);
        }

        protected internal override void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            const byte UnitColIndex = 3;
            const byte TotalColIndex = 5;

            // unit
            var headers = gridUnitData.Headers;
            templateWorksheet.Cells[RowHeaderIndex, 1, RowBodyIndex - 1, 1].Copy(worksheet.Cells[RowHeaderIndex, 1, RowBodyIndex - 1, 1]);
            var excelAddress = $"{worksheet.Cells[RowHeaderIndex, 1, RowBodyIndex - 1, UnitColIndex - 1].Address}";
            worksheet.Cells[excelAddress].Merge = true;
            const byte ColStep = 2;
            var colIndex = 3;
            var gunitColIndex = 3;
            foreach (var gunit in headers)
            {
                if (gunit.Name != "Other")
                {
                    foreach (var unit in gunit.Cells)
                    {
                        templateWorksheet.Cells[RowHeaderIndex, UnitColIndex, RowHeaderIndex, UnitColIndex + 1]
                               .Copy(worksheet.Cells[RowHeaderIndex, colIndex, 1, colIndex + 1]);

                        templateWorksheet.Cells[RowHeaderIndex + 1, UnitColIndex, RowBodyIndex - 1, UnitColIndex + 1]
                            .Copy(worksheet.Cells[RowHeaderIndex + 1, colIndex, RowBodyIndex - 1, colIndex + 1]);
                        worksheet.Cells[RowHeaderIndex + 1, colIndex].Value = unit.Name;

                        colIndex += ColStep;
                    }

                }
                templateWorksheet.Cells[RowHeaderIndex, TotalColIndex, RowBodyIndex - 1, TotalColIndex + 1]
                   .Copy(worksheet.Cells[RowHeaderIndex, colIndex, RowBodyIndex - 1, colIndex + 1]);

                excelAddress = $"{worksheet.Cells[RowHeaderIndex, gunitColIndex, RowHeaderIndex, colIndex + 1].Address}";
                worksheet.Cells[excelAddress].Merge = true;
                worksheet.Cells[RowHeaderIndex, gunitColIndex].Value = gunit.Name;

                colIndex += ColStep;
                gunitColIndex = colIndex;
            }

            //company

            colIndex++;
            _startCompanyColIndex = colIndex;
            headers = gridCompanyData.Headers;
            templateWorksheet.Cells[RowHeaderIndex, 1, RowBodyIndex - 1, 1].Copy(worksheet.Cells[RowHeaderIndex, colIndex, RowBodyIndex - 1, colIndex]);
            excelAddress = $"{worksheet.Cells[RowHeaderIndex, colIndex, RowBodyIndex - 1, colIndex + 1].Address}";
            worksheet.Cells[excelAddress].Merge = true;
            colIndex += Two;
            foreach (var country in headers)
            {
                if (country.Name != "Other")
                {
                    templateWorksheet.Cells[RowHeaderIndex, 10, RowHeaderIndex + 1, 10].Copy(worksheet.Cells[RowHeaderIndex, colIndex, RowHeaderIndex + 1, colIndex + country.Cells.Count]);
                    worksheet.Cells[RowHeaderIndex, colIndex, RowHeaderIndex + 1, colIndex + country.Cells.Count].Merge = true;
                    worksheet.Cells[RowHeaderIndex, colIndex].Value = country.Name;
                    foreach (var company in country.Cells)
                    {
                        templateWorksheet.Cells[RowBodyIndex - 1, 10].Copy(worksheet.Cells[RowHeaderIndex + Two, colIndex]);
                        worksheet.Cells[RowHeaderIndex + Two, colIndex++].Value = company.Name;
                    }
                    templateWorksheet.Cells[RowBodyIndex - 1, 10].Copy(worksheet.Cells[RowHeaderIndex + Two, colIndex]);
                    worksheet.Cells[RowHeaderIndex + Two, colIndex].Value = "Total";
                }
                else
                {
                    templateWorksheet.Cells[RowHeaderIndex, 12, RowBodyIndex - 1, 12].Copy(worksheet.Cells[RowHeaderIndex, colIndex, RowHeaderIndex + Two, colIndex]);
                    worksheet.Cells[RowHeaderIndex, colIndex, RowHeaderIndex + Two, colIndex + country.Cells.Count].Merge = true;
                    worksheet.Cells[RowHeaderIndex, colIndex++].Value = country.Name;
                }
                colIndex += country.Cells.Count;
            }

            templateWorksheet.Cells[RowHeaderIndex, 12, RowBodyIndex - 1, 12].Copy(worksheet.Cells[RowHeaderIndex, colIndex, RowHeaderIndex + Two, colIndex]);
            worksheet.Cells[RowHeaderIndex, colIndex, RowHeaderIndex + Two, colIndex].Merge = true;
            worksheet.Cells[RowHeaderIndex, colIndex].Value = "Grand Total";

            // country
            colIndex += Two;
            _startCountryColIndex = colIndex;
            headers = gridCountryData.Headers;
            templateWorksheet.Cells[RowHeaderIndex, 1, RowBodyIndex - 1, 1].Copy(worksheet.Cells[RowHeaderIndex, colIndex, RowBodyIndex - 1, colIndex]);
            excelAddress = $"{worksheet.Cells[RowHeaderIndex, colIndex, RowBodyIndex - 1, colIndex + 1].Address}";
            worksheet.Cells[excelAddress].Merge = true;
            colIndex += Two;
            foreach (var country in headers)
            {
                templateWorksheet.Cells[RowHeaderIndex, CountryColIndex, RowBodyIndex - 1, CountryColIndex + Three]
                           .Copy(worksheet.Cells[RowHeaderIndex, colIndex, RowBodyIndex - 1, colIndex + 2]);
                worksheet.Cells[RowHeaderIndex, colIndex, RowHeaderIndex + 1, colIndex + Three].Merge = true;
                worksheet.Cells[RowHeaderIndex, colIndex].Value = country.Name;
                colIndex += 4;
            }
        }

        protected internal override void RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            // unit
            int rowIndex = RowBodyIndex;
            IList<RowItem> rows = gridUnitData.Supervisors;
            IList<RowItem> totalJobTypes = gridUnitData.TotalJobTypeSupervisor;
            RenderGrandTotalRow(templateWorksheet, worksheet, ref rowIndex, gridUnitData.GrandTotal);

            RenderRows(templateWorksheet, worksheet, ref rowIndex, rows, totalJobTypes);

            RenderTotalRow(templateWorksheet, worksheet, ref rowIndex, gridUnitData.TotalSupervisor);

            rows = gridUnitData.Employees;
            totalJobTypes = gridUnitData.TotalJobTypeEmployee;

            RenderRows(templateWorksheet, worksheet, ref rowIndex, rows, totalJobTypes);

            RenderTotalRow(templateWorksheet, worksheet, ref rowIndex, gridUnitData.TotalEmployee);

            rows = gridUnitData.Others;
            totalJobTypes = gridUnitData.TotalJobTypeOthers;

            RenderRows(templateWorksheet, worksheet, ref rowIndex, rows, totalJobTypes);

            RenderUnitGrandTotalRow(templateWorksheet, worksheet, ref rowIndex, gridUnitData);

            // company
            rowIndex = RowBodyIndex;
            rows = gridCompanyData.Supervisors;
            totalJobTypes = gridCompanyData.TotalJobTypeSupervisor;
            RenderCompanySubGrandTotalRow(templateWorksheet, worksheet, ref rowIndex, gridCompanyData);
            rowIndex++;
            RenderCompanyRows(templateWorksheet, worksheet, ref rowIndex, rows, totalJobTypes);

            RenderCompanyTotalRow(templateWorksheet, worksheet, ref rowIndex, gridCompanyData.TotalSupervisor);

            rows = gridCompanyData.Employees;
            totalJobTypes = gridCompanyData.TotalJobTypeEmployee;
            RenderCompanyRows(templateWorksheet, worksheet, ref rowIndex, rows, totalJobTypes);

            RenderCompanyTotalRow(templateWorksheet, worksheet, ref rowIndex, gridCompanyData.TotalEmployee);

            rows = gridCompanyData.Others;
            totalJobTypes = gridCompanyData.TotalJobTypeOthers;
            RenderCompanyRows(templateWorksheet, worksheet, ref rowIndex, rows, totalJobTypes);

            //// country
            rowIndex = RowBodyIndex;
            rows = gridCountryData.Supervisors;
            totalJobTypes = gridCountryData.TotalJobTypeSupervisor;
            RenderCountrySubGrandTotalRow(templateWorksheet, worksheet, ref rowIndex, gridCountryData);

            RenderCountryRows(templateWorksheet, worksheet, ref rowIndex, rows, totalJobTypes);

            RenderCountryTotalRow(templateWorksheet, worksheet, ref rowIndex, gridCountryData.TotalSupervisor);

            rows = gridCountryData.Employees;
            totalJobTypes = gridCountryData.TotalJobTypeEmployee;
            RenderCountryRows(templateWorksheet, worksheet, ref rowIndex, rows, totalJobTypes);

            RenderCountryTotalRow(templateWorksheet, worksheet, ref rowIndex, gridCountryData.TotalEmployee);

            rows = gridCountryData.Others;
            totalJobTypes = gridCountryData.TotalJobTypeOthers;
            RenderCountryRows(templateWorksheet, worksheet, ref rowIndex, rows, totalJobTypes);

            RenderCountryGrandTotalRow(templateWorksheet, worksheet, ref rowIndex, gridCountryData);

        }

        protected internal override void ProcessUI(ExcelWorksheet worksheet)
        {
            var totalCols = 2;
            var colIndex = 3;
            foreach (var gunit in gridUnitData.Headers.Where(x => x.Name != "Other"))
            {
                totalCols += gunit.Cells.Count * Two;
                for (var i = colIndex; i <= totalCols; i++)
                {
                    worksheet.Column(i).OutlineLevel = 1;
                    worksheet.Column(i).Collapsed = true;
                }

                totalCols += Two;
                colIndex = totalCols + 1;
            }

            worksheet.Cells.AutoFitColumns();
            worksheet.View.FreezePanes(RowBodyIndex, 1);
        }

        private static void RenderRowJobLevelData(ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            ref int rowIndex,
            ref int colIndex,
            RowItem row)
        {
            const byte UnitColIndex = Three;
            const byte TotalColIndex = 5;
            foreach (var gunit in row.Cells)
            {
                var totalExpat = 0;
                var totalLocal = 0;

                foreach (var unit in gunit.Cells)
                {
                    if (gunit.Name != "Other")
                    {
                        templateWorksheet.Cells[RowBodyIndex, UnitColIndex, RowBodyIndex, UnitColIndex + 1]
                            .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                        worksheet.Cells[rowIndex, colIndex++].Value = unit.TotalExpat;
                        worksheet.Cells[rowIndex, colIndex++].Value = unit.TotalLocal;
                    }

                    totalExpat += unit.TotalExpat;
                    totalLocal += unit.TotalLocal;
                }

                templateWorksheet.Cells[RowBodyIndex, TotalColIndex, RowBodyIndex, TotalColIndex + 1]
                        .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                worksheet.Cells[rowIndex, colIndex++].Value = totalExpat;
                worksheet.Cells[rowIndex, colIndex++].Value = totalLocal;
            }
        }

        private static void RenderRowJobLevel(ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            ref int rowIndex,
            IList<RowItem> rows,
            RowItem jobType)
        {
            foreach (var row in rows)
            {
                if (jobType.Name.Contains(row.Name.Substring(0, Two)) || row.Name == "Other")
                {
                    var colIndex = 2;
                    templateWorksheet.Cells[RowBodyIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
                    worksheet.Cells[rowIndex, colIndex++].Value = IsMultipleData(rows, jobType.Name) ? row.Name : "Total";
                    if (IsMultipleData(rows, jobType.Name))
                    {
                        worksheet.Row(rowIndex).OutlineLevel = 1;
                        worksheet.Row(rowIndex).Collapsed = true;
                    }
                    RenderRowJobLevelData(templateWorksheet, worksheet, ref rowIndex, ref colIndex, row);
                    rowIndex++;
                }
            }
        }

        private static void RenderRowTotalJobLevelData(ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            ref int rowIndex,
            ref int colIndex,
            RowItem jobType)
        {
            const byte UnitColIndex = Three;
            const byte TotalColIndex = 5;
            foreach (var gunit in jobType.Cells)
            {
                var totalExpat = 0;
                var totalLocal = 0;

                foreach (var unit in gunit.Cells)
                {
                    if (gunit.Name != "Other")
                    {
                        templateWorksheet.Cells[RowBodyIndex, UnitColIndex, RowBodyIndex, UnitColIndex + 1]
                            .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                        worksheet.Cells[rowIndex, colIndex++].Value = unit.TotalExpat;
                        worksheet.Cells[rowIndex, colIndex++].Value = unit.TotalLocal;
                    }
                    totalExpat += unit.TotalExpat;
                    totalLocal += unit.TotalLocal;
                }

                templateWorksheet.Cells[RowBodyIndex, TotalColIndex, RowBodyIndex, TotalColIndex + 1]
                        .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                worksheet.Cells[rowIndex, colIndex++].Value = totalExpat;
                worksheet.Cells[rowIndex, colIndex++].Value = totalLocal;
            }
        }

        private static void RenderRowTotalJobLevel(ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            ref int rowIndex,
            IList<RowItem> rows,
            RowItem jobType)
        {
            if (IsMultipleData(rows, jobType.Name))
            {
                var colIndex = 2;
                templateWorksheet.Cells[RowBodyIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
                worksheet.Cells[rowIndex, colIndex++].Value = "Total";
                RenderRowTotalJobLevelData(templateWorksheet, worksheet, ref rowIndex, ref colIndex, jobType);

                rowIndex++;
            }
        }

        private static void RenderRowTotalJobTypeData(ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            ref int rowIndex,
            ref int colIndex,
            RowItem jobType)
        {
            const byte UnitColIndex = Three;
            foreach (var gunit in jobType.Cells)
            {
                var total = 0;

                foreach (var unit in gunit.Cells)
                {
                    if (gunit.Name != "Other")
                    {
                        templateWorksheet.Cells[TotalRowIndex, UnitColIndex, TotalRowIndex, UnitColIndex + 1]
                            .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                        worksheet.Cells[rowIndex, colIndex++].Value = unit.TotalExpat + unit.TotalLocal;
                        colIndex++;
                    }
                    total += unit.TotalExpat + unit.TotalLocal;
                }

                templateWorksheet.Cells[TotalRowIndex + 1, UnitColIndex, TotalRowIndex + 1, UnitColIndex + 1]
                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                worksheet.Cells[rowIndex, colIndex++].Value = total;
                colIndex++;
            }
        }

        private static void RenderRowTotalJobType(ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            ref int rowIndex,
            IList<RowItem> rows,
            RowItem jobType)
        {
            const byte UnitColIndex = 1;
            if (IsMultipleData(rows, jobType.Name))
            {
                var colIndex = 3;
                templateWorksheet.Cells[RowBodyIndex, UnitColIndex, RowBodyIndex, UnitColIndex + 1]
                             .Copy(worksheet.Cells[rowIndex, 1, rowIndex, 1]);
                worksheet.Cells[rowIndex, 1].Value = "Total " + jobType.Name;
                RenderRowTotalJobTypeData(templateWorksheet, worksheet, ref rowIndex, ref colIndex, jobType);

                rowIndex++;
            }
        }

        private static void RenderRows(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, IList<RowItem> rows, IList<RowItem> totalJobTypes)
        {
            const byte UnitColIndex = Two;

            foreach (var jobType in totalJobTypes)
            {
                templateWorksheet.Cells[TotalRowIndex, UnitColIndex, TotalRowIndex, UnitColIndex + 1]
                               .Copy(worksheet.Cells[rowIndex, 1, rowIndex, 1]);
                var excelAddress = $"{worksheet.Cells[rowIndex, 1, rowIndex + GetNumRowJobType(rows, jobType.Name), 1].Address}";
                worksheet.Cells[excelAddress].Merge = true;
                worksheet.Cells[rowIndex, 1].Value = jobType.Name;

                RenderRowJobLevel(templateWorksheet, worksheet, ref rowIndex, rows, jobType);

                RenderRowTotalJobLevel(templateWorksheet, worksheet, ref rowIndex, rows, jobType);

                RenderRowTotalJobType(templateWorksheet, worksheet, ref rowIndex, rows, jobType);
            }
        }

        private static void RenderTotalFirstRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, RowItem row, int colIndex)
        {
            const byte UnitColIndex = Two;
            foreach (var gunit in row.Cells)
            {
                var totalExpat = 0;
                var totalLocal = 0;

                foreach (var unit in gunit.Cells)
                {
                    if (gunit.Name != "Other")
                    {
                        templateWorksheet.Cells[TotalRowIndex, UnitColIndex, TotalRowIndex, UnitColIndex + 1]
                                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                        worksheet.Cells[rowIndex, colIndex++].Value = unit.TotalExpat;
                        worksheet.Cells[rowIndex, colIndex++].Value = unit.TotalLocal;
                    }
                    totalExpat += unit.TotalExpat;
                    totalLocal += unit.TotalLocal;
                }

                templateWorksheet.Cells[TotalRowIndex, UnitColIndex, TotalRowIndex, UnitColIndex + 1]
                                .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                worksheet.Cells[rowIndex, colIndex++].Value = totalExpat;
                worksheet.Cells[rowIndex, colIndex++].Value = totalLocal;
            }

            rowIndex++;
        }

        private static void RenderTotalSecondRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, RowItem row, int colIndex)
        {
            const byte UnitColIndex = Three;
            foreach (var gunit in row.Cells)
            {
                var total = 0;
                foreach (var unit in gunit.Cells)
                {
                    if (gunit.Name != "Other")
                    {
                        templateWorksheet.Cells[TotalRowIndex, UnitColIndex, TotalRowIndex, UnitColIndex + 1]
                                        .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                        worksheet.Cells[rowIndex, colIndex++].Value = unit.TotalExpat + unit.TotalLocal;
                        colIndex++;
                    }
                    total += unit.TotalExpat + unit.TotalLocal;
                }

                templateWorksheet.Cells[GrandTotalRowIndex, UnitColIndex, GrandTotalRowIndex, UnitColIndex + 1]
                                .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                worksheet.Cells[rowIndex, colIndex++].Value = total;
                colIndex++;
            }
            rowIndex++;
        }

        private static void RenderTotalRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, RowItem row)
        {
            var colIndex = 1;
            templateWorksheet.Cells[TotalRowIndex, 1, TotalRowIndex + 1, Two].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex + 1]);
            worksheet.Cells[rowIndex, colIndex++].Value = row.Name;
            colIndex++;
            RenderTotalFirstRow(templateWorksheet, worksheet, ref rowIndex, row, colIndex);
            colIndex = Three;
            RenderTotalSecondRow(templateWorksheet, worksheet, ref rowIndex, row, colIndex);
        }

        private static void RenderGrandTotalFirstRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, RowItem row, ref int colIndex)
        {
            const byte UnitColIndex = Two;
            const byte TotalColIndex = 4;
            foreach (var gunit in row.Cells)
            {
                var totalExpat = 0;
                var totalLocal = 0;

                foreach (var unit in gunit.Cells)
                {
                    if (gunit.Name != "Other")
                    {
                        templateWorksheet.Cells[GrandTotalRowIndex, UnitColIndex, GrandTotalRowIndex, UnitColIndex + 1]
                            .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                        worksheet.Cells[rowIndex, colIndex++].Value = unit.TotalExpat;
                        worksheet.Cells[rowIndex, colIndex++].Value = unit.TotalLocal;
                    }
                    totalExpat += unit.TotalExpat;
                    totalLocal += unit.TotalLocal;
                }

                templateWorksheet.Cells[GrandTotalRowIndex, TotalColIndex, GrandTotalRowIndex, TotalColIndex + 1]
                        .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                worksheet.Cells[rowIndex, colIndex++].Value = totalExpat;
                worksheet.Cells[rowIndex, colIndex++].Value = totalLocal;
            }
            rowIndex++;
            colIndex = Three;
        }

        private static void RenderGrandTotalSecondRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, RowItem row, ref int colIndex)
        {
            const byte UnitColIndex = Two;
            const byte TotalColIndex = 4;
            foreach (var gunit in row.Cells)
            {
                var total = 0;

                foreach (var unit in gunit.Cells)
                {
                    if (gunit.Name != "Other")
                    {
                        templateWorksheet.Cells[GrandTotalRowIndex, UnitColIndex, GrandTotalRowIndex, UnitColIndex + 1]
                            .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                        worksheet.Cells[rowIndex, colIndex++].Value = unit.TotalExpat + unit.TotalLocal;
                        total += unit.TotalExpat + unit.TotalLocal;
                        colIndex++;
                    }
                }

                templateWorksheet.Cells[GrandTotalRowIndex, TotalColIndex, GrandTotalRowIndex, TotalColIndex + 1]
                        .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
                worksheet.Cells[rowIndex, colIndex++].Value = total;
                colIndex++;
            }
            rowIndex++;
        }

        private static void RenderGrandTotalRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, RowItem row)
        {
            const byte UnitColIndex = Two;

            var colIndex = 1;
            templateWorksheet.Cells[GrandTotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
            var excelAddress = $"{worksheet.Cells[rowIndex, 1, rowIndex + 1, UnitColIndex].Address}";
            worksheet.Cells[excelAddress].Merge = true;
            worksheet.Cells[rowIndex, colIndex++].Value = "Sub Total";
            colIndex++;
            RenderGrandTotalFirstRow(templateWorksheet, worksheet, ref rowIndex, row, ref colIndex);
            RenderGrandTotalSecondRow(templateWorksheet, worksheet, ref rowIndex, row, ref colIndex);
        }

        private void RenderCountryRows(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, IList<RowItem> rows, IList<RowItem> totalJobTypes)
        {
            foreach (var jobType in totalJobTypes)
            {
                var colIndex = _startCountryColIndex;
                templateWorksheet.Cells[TotalRowIndex, Two, TotalRowIndex, Three]
                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex]);
                var excelAddress = $"{worksheet.Cells[rowIndex, colIndex, rowIndex + GetNumRowJobType(rows, jobType.Name), colIndex].Address}";
                worksheet.Cells[excelAddress].Merge = true;
                worksheet.Cells[rowIndex, colIndex].Value = jobType.Name;

                RenderCountryRowsJobLevel(templateWorksheet, worksheet, ref rowIndex, rows, jobType);
                RenderCountryRowsTotalJobLevel(templateWorksheet, worksheet, ref rowIndex, rows, jobType);
                RenderCountryRowsTotalJobType(templateWorksheet, worksheet, ref rowIndex, rows, jobType);
            }
        }

        private void RenderCompanyRows(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, IList<RowItem> rows, IList<RowItem> totalJobTypes)
        {
            foreach (var jobType in totalJobTypes)
            {
                var colIndex = _startCompanyColIndex;
                templateWorksheet.Cells[TotalRowIndex, Two, TotalRowIndex, Three]
                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex]);
                var excelAddress = $"{worksheet.Cells[rowIndex, colIndex, rowIndex + GetNumRowJobType(rows, jobType.Name), colIndex].Address}";
                worksheet.Cells[excelAddress].Merge = true;
                worksheet.Cells[rowIndex, colIndex].Value = jobType.Name;

                RenderCompanyRowsJobLevel(templateWorksheet, worksheet, ref rowIndex, rows, jobType);
                RenderCompanyRowsTotalJobLevel(templateWorksheet, worksheet, ref rowIndex, rows, jobType);
                RenderCompanyRowsTotalJobType(templateWorksheet, worksheet, ref rowIndex, rows, jobType);
            }
        }

        private void RenderCountryRowsJobLevel(ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            ref int rowIndex,
            IList<RowItem> rows,
            RowItem jobType)
        {
            foreach (var row in rows)
            {
                if (jobType.Name.Contains(row.Name[..Two]))
                {
                    var colIndex = _startCountryColIndex + 1;
                    templateWorksheet.Cells[RowBodyIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);

                    var isMultiRow = IsMultipleData(rows, jobType.Name);
                    worksheet.Cells[rowIndex, colIndex++].Value = isMultiRow ? row.Name : "Total";
                    if (isMultiRow)
                    {
                        worksheet.Row(rowIndex).OutlineLevel = 1;
                        worksheet.Row(rowIndex).Collapsed = true;
                    }
                    RenderCountryRowsJobLevelData(templateWorksheet, worksheet, ref rowIndex, colIndex, row, isMultiRow);

                    rowIndex++;
                }
            }
        }

        private void RenderCompanyRowsJobLevel(ExcelWorksheet templateWorksheet,
           ExcelWorksheet worksheet,
           ref int rowIndex,
           IList<RowItem> rows,
           RowItem jobType)
        {
            foreach (var row in rows)
            {
                if (jobType.Name.Contains(row.Name[..Two]))
                {
                    var colIndex = _startCompanyColIndex + 1;
                    templateWorksheet.Cells[RowBodyIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);

                    var isMultiRow = IsMultipleData(rows, jobType.Name);
                    worksheet.Cells[rowIndex, colIndex++].Value = isMultiRow ? row.Name : "Total";
                    if (isMultiRow)
                    {
                        worksheet.Row(rowIndex).OutlineLevel = 1;
                        worksheet.Row(rowIndex).Collapsed = true;
                    }
                    RenderCompanyRowsJobLevelData(templateWorksheet, worksheet, ref rowIndex, colIndex, row, isMultiRow);

                    rowIndex++;
                }
            }
        }

        private static void RenderCountryRowsJobLevelData(ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            ref int rowIndex,
            int colIndex,
            RowItem row,
            bool isMultiRow)
        {
            foreach (var country in row.Cells)
            {
                foreach (var unit in country.Cells)
                {
                    if (isMultiRow)
                    {
                        templateWorksheet.Cells[RowBodyIndex, CountryColIndex, RowBodyIndex, CountryColIndex]
                            .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex]);
                    }
                    else
                    {
                        templateWorksheet.Cells[TotalRowIndex, CountryColIndex, TotalRowIndex, CountryColIndex]
                            .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex]);
                    }

                    worksheet.Cells[rowIndex, colIndex++].Value = unit.Total;
                }
            }
        }

        private static void RenderCompanyRowsJobLevelData(ExcelWorksheet templateWorksheet,
           ExcelWorksheet worksheet,
           ref int rowIndex,
           int colIndex,
           RowItem row,
           bool isMultiRow)
        {
            int grandTotal = 0;
            foreach (var country in row.Cells)
            {
                int totalSub = 0;
                foreach (var company in country.Cells)
                {
                    templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
                    worksheet.Cells[rowIndex, colIndex++].Value = company.Total;
                    totalSub += company.Total;
                }
                templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumber(totalSub);
                grandTotal += totalSub;
            }
            templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex].Value = RenderNumber(grandTotal);
        }

        private void RenderCountryRowsTotalJobLevel(ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            ref int rowIndex,
            IList<RowItem> rows,
            RowItem jobType)
        {
            if (IsMultipleData(rows, jobType.Name))
            {
                var colIndex = _startCountryColIndex + 1;
                templateWorksheet.Cells[RowBodyIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
                worksheet.Cells[rowIndex, colIndex++].Value = "Total";
                RenderCountryRowsTotalJobLevelData(templateWorksheet, worksheet, ref rowIndex, colIndex, jobType);
                rowIndex++;
            }
        }

        private void RenderCompanyRowsTotalJobLevel(ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            ref int rowIndex,
            IList<RowItem> rows,
            RowItem jobType)
        {
            if (IsMultipleData(rows, jobType.Name))
            {
                var colIndex = _startCompanyColIndex + 1;
                templateWorksheet.Cells[RowBodyIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
                worksheet.Cells[rowIndex, colIndex++].Value = "Total";
                RenderCompanyRowsTotalJobLevelData(templateWorksheet, worksheet, ref rowIndex, colIndex, jobType);
                rowIndex++;
            }
        }

        private static void RenderCountryRowsTotalJobLevelData(ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            ref int rowIndex,
            int colIndex,
            RowItem jobType)
        {
            foreach (var country in jobType.Cells)
            {
                var total = 0;
                foreach (var unit in country.Cells)
                {
                    templateWorksheet.Cells[TotalRowIndex, CountryColIndex, TotalRowIndex, CountryColIndex]
                        .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex]);
                    worksheet.Cells[rowIndex, colIndex++].Value = unit.Total;
                    total += unit.Total;
                }
            }
        }

        private static void RenderCompanyRowsTotalJobLevelData(ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            ref int rowIndex,
            int colIndex,
            RowItem jobType)
        {
            var grandTotal = 0;
            foreach (var country in jobType.Cells)
            {
                var totalCompany = 0;
                foreach (var company in country.Cells)
                {

                    templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
                    worksheet.Cells[rowIndex, colIndex++].Value = company.Total;
                    totalCompany += company.Total;
                }
                templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumber(totalCompany);
                grandTotal += totalCompany;
            }
            templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex].Value = RenderNumber(grandTotal);
        }

        private static void RenderCountryRowsTotalJobTypeData(ExcelWorksheet templateWorksheet,
           ExcelWorksheet worksheet,
           ref int rowIndex,
           int colIndex,
           RowItem jobType)
        {
            foreach (var country in jobType.Cells)
            {
                templateWorksheet.Cells[TotalRowIndex, CountryColIndex, TotalRowIndex, CountryColIndex + Three]
                     .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + Three]);
                worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + Three].Merge = true;
                worksheet.Cells[rowIndex, colIndex].Value = country.Cells.Sum(unit => unit.Total);
                colIndex += Four;
            }
        }

        private static void RenderCompanyRowsTotalJobTypeData(ExcelWorksheet templateWorksheet,
           ExcelWorksheet worksheet,
           ref int rowIndex,
           int colIndex,
           RowItem jobType)
        {
            int grandTotal = 0;
            foreach (var country in jobType.Cells)
            {

                templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);

                worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + country.Cells.Count].Merge = true;
                worksheet.Cells[rowIndex, colIndex].Value = RenderNumber(country.Cells.Sum(unit => unit.Total));
                colIndex += country.Cells.Count + 1;
                grandTotal += country.Cells.Sum(unit => unit.Total);
            }
            templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex].Value = RenderNumber(grandTotal);
        }

        private void RenderCountryRowsTotalJobType(ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            ref int rowIndex,
            IList<RowItem> rows,
            RowItem jobType)
        {
            if (IsMultipleData(rows, jobType.Name))
            {
                var colIndex = _startCountryColIndex;
                templateWorksheet.Cells[TotalRowIndex, CountryJobLevelColIndex, TotalRowIndex, CountryJobLevelColIndex + 1]
                             .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                var excelAddress = $"{worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Address}";
                worksheet.Cells[excelAddress].Merge = true;
                worksheet.Cells[rowIndex, colIndex].Value = "Total " + jobType.Name;
                colIndex += Two;
                RenderCountryRowsTotalJobTypeData(templateWorksheet, worksheet, ref rowIndex, colIndex, jobType);

                rowIndex++;
            }
        }

        private void RenderCompanyRowsTotalJobType(ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            ref int rowIndex,
            IList<RowItem> rows,
            RowItem jobType)
        {
            if (IsMultipleData(rows, jobType.Name))
            {
                var colIndex = _startCompanyColIndex;

                templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);

                var excelAddress = $"{worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Address}";
                worksheet.Cells[excelAddress].Merge = true;
                worksheet.Cells[rowIndex, colIndex].Value = "Total " + jobType.Name;
                colIndex += Two;
                RenderCompanyRowsTotalJobTypeData(templateWorksheet, worksheet, ref rowIndex, colIndex, jobType);
                rowIndex++;
            }
        }

        private static void RenderCountryTotalFirstRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, RowItem row, int colIndex)
        {
            foreach (var country in row.Cells)
            {
                foreach (var unit in country.Cells)
                {
                    templateWorksheet.Cells[TotalRowIndex, CountryColIndex, TotalRowIndex, CountryColIndex]
                               .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex]);
                    worksheet.Cells[rowIndex, colIndex++].Value = unit.Total;
                }
            }
            rowIndex++;
        }

        private static void RenderCompanyTotalFirstRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, RowItem row, int colIndex)
        {
            int grandTotal = 0;

            foreach (var country in row.Cells)
            {
                int totalCompany = 0;
                foreach (var company in country.Cells)
                {
                    templateWorksheet.Cells[TotalRowIndex, CountryColIndex, TotalRowIndex, CountryColIndex]
                               .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex]);
                    worksheet.Cells[rowIndex, colIndex++].Value = company.Total;
                    totalCompany += company.Total;
                }
                templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
                worksheet.Cells[rowIndex, colIndex++].Value = RenderNumber(totalCompany);
                grandTotal += totalCompany;
            }
            templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex].Value = RenderNumber(grandTotal);
            rowIndex++;
        }

        private static void RenderCountryTotalSecondRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, RowItem row, int colIndex)
        {
            foreach (var country in row.Cells)
            {
                var total = 0;
                foreach (var unit in country.Cells)
                {
                    total += unit.Total;
                }
                templateWorksheet.Cells[TotalRowIndex + 1, CountryColIndex, TotalRowIndex + 1, CountryColIndex + Three]
                               .Copy(worksheet.Cells[rowIndex, colIndex]);
                worksheet.Cells[rowIndex, colIndex].Value = total;
                colIndex += Four;
            }
            rowIndex++;
        }

        private static void RenderCompanyTotalSecondRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, RowItem row, int colIndex)
        {
            int grandTotal = 0;
            foreach (var country in row.Cells)
            {
                var total = 0;
                foreach (var unit in country.Cells)
                {
                    total += unit.Total;
                }
                templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);

                if (country.Name != "Other")
                {
                    worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
                }
                worksheet.Cells[rowIndex, colIndex].Value = RenderNumber(total);
                colIndex += country.Cells.Count + 1;
                grandTotal += total;
            }
            templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex].Value = RenderNumber(grandTotal);
            rowIndex++;
        }

        private void RenderCountryTotalRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, RowItem row)
        {
            var colIndex = _startCountryColIndex;
            templateWorksheet.Cells[TotalRowIndex, 1, TotalRowIndex + 1, Two].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
            worksheet.Cells[rowIndex, colIndex++].Value = row.Name;
            colIndex++;
            RenderCountryTotalFirstRow(templateWorksheet, worksheet, ref rowIndex, row, colIndex);
            colIndex = _startCountryColIndex + Two;
            RenderCountryTotalSecondRow(templateWorksheet, worksheet, ref rowIndex, row, colIndex);
        }

        private void RenderCompanyTotalRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, RowItem row)
        {
            var colIndex = _startCompanyColIndex;
            templateWorksheet.Cells[TotalRowIndex, 1, TotalRowIndex + 1, Two].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
            worksheet.Cells[rowIndex, colIndex++].Value = row.Name;
            colIndex++;
            RenderCompanyTotalFirstRow(templateWorksheet, worksheet, ref rowIndex, row, colIndex);
            colIndex = _startCompanyColIndex + Two;
            RenderCompanyTotalSecondRow(templateWorksheet, worksheet, ref rowIndex, row, colIndex);
        }

        private void RenderCountryGrandTotalRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, GridData data)
        {
            var colIndex = _startCountryColIndex;
            templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[$"{worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Address}"].Merge = true;
            worksheet.Cells[rowIndex, colIndex].Value = "Grand Total";
            colIndex = _startCountryColIndex + Two;

            templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
            var excelAddressTotalGrand = $"{worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + (data.GrandTotal.Cells.Count * 4 - 1)].Address}";
            worksheet.Cells[excelAddressTotalGrand].Merge = true;
            var totalGrand = 0;
            foreach (var row in from item in data.GrandTotal.Cells
                                from row in item.Cells
                                select row)
            {
                totalGrand += row.Total;
            }

            worksheet.Cells[excelAddressTotalGrand].Value = totalGrand;
        }

        private static void RenderUnitGrandTotalRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, GridData data)
        {
            var colIndex = 1;
            templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[$"{worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Address}"].Merge = true;
            worksheet.Cells[rowIndex, colIndex].Value = "Grand Total";
            colIndex = Three;

            var numGrand = data.GrandTotal.Cells.Sum(x => x.Cells.Count) * 2 + 1;

            templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
            var excelAddressTotalGrand = $"{worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + numGrand].Address}";
            worksheet.Cells[excelAddressTotalGrand].Merge = true;
            var totalGrand = 0;
            foreach (var row in from item in data.GrandTotal.Cells
                                from row in item.Cells
                                select row)
            {
                totalGrand += row.TotalLocal + row.TotalExpat;
            }

            worksheet.Cells[excelAddressTotalGrand].Value = totalGrand;
        }

        private void RenderCountrySubGrandTotalRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, GridData data)
        {
            var colIndex = _startCountryColIndex;
            templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
            var excelAddress = $"{worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex + 1].Address}";
            worksheet.Cells[excelAddress].Merge = true;
            worksheet.Cells[rowIndex, colIndex++].Value = data.GrandTotal.Name;
            colIndex++;
            RenderCountryGrandTotalFirstRow(templateWorksheet, worksheet, ref rowIndex, data, colIndex);
            colIndex = _startCountryColIndex + Two;
            RenderCountryGrandTotalSecondRow(templateWorksheet, worksheet, ref rowIndex, colIndex, data.GrandTotal);
        }

        private void RenderCompanySubGrandTotalRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, GridData data)
        {
            var colIndex = _startCompanyColIndex;
            templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
            var excelAddress = $"{worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex + 1].Address}";
            worksheet.Cells[excelAddress].Merge = true;
            worksheet.Cells[rowIndex, colIndex++].Value = data.GrandTotal.Name;
            colIndex++;
            RenderCompanyGrandTotalFirstRow(templateWorksheet, worksheet, ref rowIndex, data, colIndex);
        }

        private static void RenderCountryGrandTotalFirstRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, GridData data, int colIndex)
        {
            int indexSup = 0;
            foreach (var country in data.TotalSupervisor.Cells)
            {
                int indexUnit = 0;
                foreach (var unit in country.Cells)
                {
                    templateWorksheet.Cells[TotalRowIndex, CountryColIndex]
                          .Copy(worksheet.Cells[rowIndex, colIndex]);
                    worksheet.Cells[rowIndex, colIndex++].Value = unit.Total + data.TotalEmployee.Cells[indexSup].Cells[indexUnit].Total;
                    indexUnit++;
                }
                indexSup++;
            }

            rowIndex++;
        }

        private static void RenderCompanyGrandTotalFirstRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, GridData data, int colIndex)
        {
            int indexSup = 0;
            int grandTotalSup = 0;
            foreach (var country in data.TotalSupervisor.Cells)
            {
                int indexUnit = 0;
                int totalSub = 0;
                if (country.Name != "Other")
                {
                    foreach (var company in country.Cells)
                    {
                        templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
                        worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
                        worksheet.Cells[rowIndex, colIndex++].Value = company.Total + data.TotalEmployee.Cells[indexSup].Cells[indexUnit].Total;
                        totalSub += company.Total + data.TotalEmployee.Cells[indexSup].Cells[indexUnit].Total;
                        indexUnit++;
                    }
                    templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
                    worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
                    worksheet.Cells[rowIndex, colIndex++].Value = RenderNumber(totalSub);
                }
                else
                {
                    int totalUnitOther = 0;
                    foreach (var company in country.Cells)
                    {
                        indexUnit++;
                        totalUnitOther += company.Total + data.TotalEmployee.Cells[indexSup].Cells[indexUnit].Total;
                    }
                    templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
                    worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
                    worksheet.Cells[rowIndex, colIndex++].Value = RenderNumber(totalUnitOther);
                }
                grandTotalSup += totalSub;
                indexSup++;
            }
            templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
            worksheet.Cells[rowIndex, colIndex].Value = RenderNumber(grandTotalSup);
            rowIndex++;
        }

        private static void RenderCountryGrandTotalSecondRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, int colIndex, RowItem row)
        {
            foreach (var country in row.Cells)
            {
                templateWorksheet.Cells[TotalRowIndex + 1, CountryColIndex, TotalRowIndex + 1, CountryColIndex + Three]
                               .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex]);
                worksheet.Cells[rowIndex, colIndex].Value = country.Cells.Sum(unit => unit.Total);
                colIndex += Four;
            }
            rowIndex++;
        }

        private static bool IsMultipleData(IList<RowItem> rows, string jobType) => rows.Where(x => jobType.Contains(x.Name.Substring(0, Two))).ToList().Count > 1 || (jobType == "Other" && rows.LastOrDefault().Cells.Count > 1);

        private static int GetNumRowJobType(IList<RowItem> rows, string jobType)
        {
            var countRow = rows.Where(x => jobType.Contains(x.Name.Substring(0, Two))).ToList().Count;
            return IsMultipleData(rows, jobType) ? countRow : 0;
        }

        private static string RenderNumber(int num) => num != 0 ? num.ToString() : string.Empty;
    }
}