namespace GO.Website.Criteria.Report.Manpower
{
    using Fanex.Data.Repository;

    public class GetGroupManpowerReportCriteria : CriteriaBase
    {
        public int UserId { get; set; }
        public short SiteId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public override string GetSettingKey() => "Report_ManpowerForecast_Get";

        public override bool IsValid() => UserId > 0 && SiteId > 0;
    }
}