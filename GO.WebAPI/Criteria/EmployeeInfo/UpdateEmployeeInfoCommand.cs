namespace GO.WebAPI.Criteria.EmployeeInfo
{
    using System.Collections.Generic;
    using Fanex.Data;
    using GO.Core.Models;
    using GO.WebAPI.Models;

    public class UpdateEmployeeInfoCommand : BaseNonQueryCommand
    {
        public short CompanyId { get; set; }
        public string EmployeeInfoXml { get; set; }

        [SpParam(Ignored = true)]
        public IList<EmployeeInfo> EmployeeInfos { get; set; }

        public override string GetSettingKey() => "HRM_SyncEmployeeInfo";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(EmployeeInfoXml) && CompanyId > 0;
    }
}