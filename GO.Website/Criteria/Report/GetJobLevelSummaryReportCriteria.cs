namespace GO.Website.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class GetJobLevelSummaryReportCriteria : CriteriaBase
    {
        public DateTime JoinedDate { get; set; }
        public override string GetSettingKey() => "Report_JobLevelSummary_Get";

        public override bool IsValid() => true;
    }
}