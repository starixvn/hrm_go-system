namespace GO.WebAPI.Criteria.User.V1
{
    using Fanex.Data;
    using GO.Core.Models;

    public class CreateUserCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string BCryptPassword { get; set; }

        public int CreatedBy { get; set; }

        [SpParam(Ignored = true)]
        public string Token { get; set; }

        public override string GetSettingKey() => "API_User_Insert_V1";

        public override bool IsValid() => SiteId > 0 && CreatedBy > 0;
    }
}