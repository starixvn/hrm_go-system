namespace GO.WebAPI.Criteria.User.V1
{
    using Fanex.Data.Repository;

    public class GetUserByUsernameCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey() => "API_User_GetByUsername_V1";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Username);
    }
}