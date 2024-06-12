namespace GO.WebAPI.Criteria.User.V1
{
    using GO.Core.Models;

    public class UpdateUserInfoCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public int UserId { get; set; }
        public string Code { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "API_User_Update_V1";

        public override bool IsValid() => UserId > 0 && SiteId > 0 && ModifiedBy > 0;
    }
}