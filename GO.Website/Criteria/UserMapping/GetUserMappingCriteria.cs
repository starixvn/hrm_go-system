namespace GO.Website.Criteria.UserMapping
{
    using Fanex.Data.Repository;

    public class GetUserMappingCriteria : CriteriaBase
    {
        public string Keyword { get; set; }

        public override string GetSettingKey() => "UserMapping_Get";

        public override bool IsValid() => true;
    }
}