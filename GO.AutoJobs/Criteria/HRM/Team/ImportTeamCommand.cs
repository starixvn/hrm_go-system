namespace GO.AutoJobs.Criteria.HRM
{
    using GO.Core.Models;

    public class ImportTeamCommand : BaseNonQueryCommand
    {
        public short CompanyId { get; set; }

        public string Value { get; set; }

        public override string GetSettingKey() => "AutoJob_Team_Import";

        public override bool IsValid() => CompanyId > 0 && !string.IsNullOrWhiteSpace(Value);
    }
}