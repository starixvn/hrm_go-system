using System.Collections.Generic;

namespace GO.Website.Models.Report.HRA;

public class HraReportModel
{
    public List<UnitReport> ReportByUnit { get; set; }
    public List<UnitReport> ReportByCompany { get; set; }
    public List<UnitReport> ReportByDetails { get; set; }
}

public class UnitReport
{
    public string Unit { get; set; }
    public string Company { get; set; }
    public string Country { get; set; }

    public IEnumerable<ReportModel> Details { get; set; }
}

public class CompanyReport
{

    public List<ReportModel> Details { get; set; }
}

public class DetailsReport
{
    public string Company { get; set; }
   // public string Country { get; set; }
   // public string Country { get; set; }
    public List<ReportModel> Details { get; set; }
}