using System.Linq;
using GO.Website.Models.Report.HRA;

namespace GO.Website.Services.ExportExcel;

using System.Drawing;
using System.IO;
using OfficeOpenXml;

public class HraReportExcel(string templatePath, HraReportModel hrAs)
{
    private const int StartColumn = 3; // C column
    private const int PropertyNumber = 4; 

    private void RenderUnitSheet(ExcelWorksheet worksheet)
    {
        var sheetData = hrAs.ReportByUnit;
        const int startDataRow = 2;
        for (var col = 0; col < sheetData.Count; col++)
        {
            var data = sheetData[col];
            var colIndex = StartColumn + col;
            worksheet.Cells[1, colIndex].Value = data.Unit; // header
            RenderValue(worksheet, startDataRow, data, colIndex);
        }
    }

    private void RenderCompanySheet(ExcelWorksheet worksheet)
    {
        var sheetData = hrAs.ReportByCompany;
        const int startDataRow = 3;
        string previousCountry = null;
        var startMergeColIdx = 0;
        for (var col = 0; col < sheetData.Count; col++)
        {
            var data = sheetData[col];
            var colIndex = StartColumn + col;
            if (data.Country == previousCountry)  // Merge country cells if necessary
            {
                worksheet.Cells[1, startMergeColIdx, 1, colIndex].Merge = true;
            }
            else
            {
                startMergeColIdx = colIndex;
                worksheet.Cells[1, colIndex].Value = data.Country; // header
            }
            worksheet.Cells[2, colIndex].Value = data.Company; // header
            RenderValue(worksheet, startDataRow, data, colIndex);
            previousCountry = data.Country;
        }
    }

    private static void RenderValue(ExcelWorksheet worksheet, int startRow, UnitReport unitData, int colIdx)
    {
        for (var month = 0; month < 12; month++)
        {
            var rowIdx = month * PropertyNumber + startRow;
            var dataByMonth = unitData.Details.FirstOrDefault(x => x.Month == month + 1) ?? new ReportModel();
            worksheet.Cells[rowIdx + 0, colIdx].Value = dataByMonth.NewHire;
            worksheet.Cells[rowIdx + 1, colIdx].Value = dataByMonth.ProbationResign;
            worksheet.Cells[rowIdx + 2, colIdx].Value = dataByMonth.ConfirmedResign;
            worksheet.Cells[rowIdx + 3, colIdx].Value = dataByMonth.CurrentHeadCount;

            if ((month + 1) % 2 != 0) continue;
            var backgroundColor = Color.FromArgb(217, 231, 253);
            worksheet.Cells[rowIdx, colIdx, rowIdx + 3, colIdx].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            worksheet.Cells[rowIdx, colIdx, rowIdx + 3, colIdx].Style.Fill.BackgroundColor.SetColor(backgroundColor);
        }
    }
    
    public byte[] Export()
    {
        using var package = new ExcelPackage(new FileInfo(templatePath));
        var worksheet1 = package.Workbook.Worksheets[0];
        var worksheet2 = package.Workbook.Worksheets[1];

        RenderUnitSheet(worksheet1);
        RenderCompanySheet(worksheet2);
        //ProcessUi(worksheet1);
        //ProcessUi(worksheet2);

        return package.GetAsByteArray();
    }
}
