namespace GO.AutoJobs.Criteria.HRM
{
    using Fanex.Data.Repository;

    public class GetSectionLastModifiedDateByCompanyIdCriteria : CriteriaBase
    {
        public short CompanyId { get; set; }

        public override string GetSettingKey() => "AutoJob_Section_GetLastModifiedDateByCompanyId";

        public override bool IsValid() => CompanyId > 0;
    }
}