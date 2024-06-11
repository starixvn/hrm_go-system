namespace GO.Website.Criteria.UserMapping
{
    using GO.Core.Models;

    public class DeleteUserMappingCommand : BaseNonQueryCommand
    {
        public int UserId { get; set; }
        public int LinkToUserId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "UserMapping_Delete";

        public override bool IsValid() => UserId > 0 && LinkToUserId > 0 && ModifiedBy > 0;
    }
}