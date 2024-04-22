namespace GO.AutoJobs.Criteria.HRM
{
    using Fanex.Data.Repository;

    public class GetTeamLastModifiedDateByCompanyIdCriteria : CriteriaBase
    {
        public short CompanyId { get; set; }

        public override string GetSettingKey() => "AutoJob_Team_GetLastModifiedDateByCompanyId";

        public override bool IsValid() => CompanyId > 0;
    }
}