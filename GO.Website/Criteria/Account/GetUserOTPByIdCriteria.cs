namespace GO.Website.Criteria.Account
{
    using Fanex.Data.Repository;

    public class GetUserOTPByIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }
        public short SiteId { get; set; }
        public short MethodId { get; set; }

        public override string GetSettingKey() => "UserTwoFA_GetById";

        public override bool IsValid() => UserId > 0 && SiteId > 0;
    }
}