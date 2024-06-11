namespace GO.Website.Criteria.UserMapping
{
    using Fanex.Data.Repository;

    public class SearchUserCriteria : CriteriaBase
    {
        public string Keyword { get; set; }
        public short? CompanyId { get; set; }
        public int? ExcludeUserId { get; set; }
        public byte? Type { get; set; }

        public override string GetSettingKey() => "UserMapping_Search";

        public override bool IsValid() => true;
    }
}