namespace GO.AutoJobs.Criteria.HRM
{
    using Fanex.Data.Repository;

    public class GetEmployeeLastModifiedDateByCompanyIdCriteria : CriteriaBase
    {
        public short CompanyId { get; set; }

        public override string GetSettingKey() => "AutoJob_Employee_GetLastModifiedDateByCompanyId";

        public override bool IsValid() => CompanyId > 0;
    }
}