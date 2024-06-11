namespace GO.Website.Criteria.UserMapping
{
    using GO.Core.Models;

    public class CreateUserMappingCommand : BaseNonQueryCommand
    {
        public int UserId { get; set; }
        public int LinkToUserId { get; set; }
        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "UserMapping_Insert";

        public override bool IsValid() => UserId > 0 && LinkToUserId > 0 && CreatedBy > 0;
    }
}