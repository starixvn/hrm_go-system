using Fanex.Data.Repository;

namespace GO.WebAPI.Criteria.User;

public class GetSiteAccessByUserIdCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public override string GetSettingKey() => "API_User_GetSites";

    public override bool IsValid() => UserId > 0;
}