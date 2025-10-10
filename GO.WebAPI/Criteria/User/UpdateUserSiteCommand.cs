using GO.Core.Models;

namespace GO.WebAPI.Criteria.User;

public class UpdateUserSiteCommand : BaseNonQueryCommand
{
    public short SiteId { get; set; }
    public int UserId { get; set; }
    public bool IsActive { get; set; }
    public int ModifiedBy { get; set; }

    public override string GetSettingKey() => "API_User_UpdateUserSite";

    public override bool IsValid() => SiteId > 0 && UserId > 0 && ModifiedBy > 0;
}