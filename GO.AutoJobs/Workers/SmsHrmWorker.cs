namespace GO.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.AutoJobs.Constants;
    using GO.AutoJobs.Helpers;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Services;
    using Microsoft.Extensions.Logging;

    public class SmsHrmWorker : BaseWorker
    {
        private readonly ISyncDataFromDBService syncDataService;

        public SmsHrmWorker(
            ISyncDataFromDBService syncDataService,
            List<JobSettings> jobSettings,
            ILogger<SmsHrmWorker> serilog,
            Fanex.Logging.ILogger logger) : base(serilog, logger)
        {
            this.syncDataService = syncDataService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.SyncDataFromSMS);
        }

        public override async Task DoWorkAsync()
        {
            await syncDataService.SyncUnitAsync(jobSetting.CompanyId, jobSetting.DbName);
            await syncDataService.SyncDepartmentAsync(jobSetting.CompanyId, jobSetting.DbName);
            await syncDataService.SyncSectionAsync(jobSetting.CompanyId, jobSetting.DbName);
            await syncDataService.SyncTeamAsync(jobSetting.CompanyId, jobSetting.DbName);
            await syncDataService.SyncJobTitleAsync(jobSetting.CompanyId, jobSetting.DbName);
            await syncDataService.SyncJobLevelAsync(jobSetting.CompanyId, jobSetting.DbName);
            await syncDataService.SyncEmployeeInfoAsync(jobSetting.CompanyId, jobSetting.DbName);
        }
    }
}