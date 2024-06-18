namespace GO.WebAPI.Criteria.User
{
    using GO.Core.Models;

    public class UpdateUserPasswordCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public int UserId { get; set; }
        public string BCryptPassword { get; set; }
        public bool IsResetPassword { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "API_User_UpdatePassword";

        public override bool IsValid() => UserId > 0 && SiteId > 0 && !string.IsNullOrWhiteSpace(BCryptPassword) && ModifiedBy > 0;
    }
}