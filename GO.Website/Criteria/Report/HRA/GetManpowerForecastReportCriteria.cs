namespace GO.Website.Criteria.Report.HRA;

using Fanex.Data.Repository;

public class GetHRAReportCriteria : CriteriaBase
{
    public int Year { get; set; }

    public override string GetSettingKey() => "Report_HRA_Get";

    public override bool IsValid() => Year > 2000;
}