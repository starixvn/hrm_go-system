namespace GO.AutoJobs.Services
{
    using System.Threading.Tasks;
    using GO.Core.Models;

    public interface ISyncDataFromDBService
    {
        Task<CommandResult> SyncEmployeeInfoAsync(short companyId, string dbName);

        Task<CommandResult> SyncTeamAsync(short companyId, string dbName);

        Task<CommandResult> SyncSectionAsync(short companyId, string dbName);

        Task<CommandResult> SyncDepartmentAsync(short companyId, string dbName);

        Task<CommandResult> SyncUnitAsync(short companyId, string dbName);

        Task<CommandResult> SyncJobTitleAsync(short companyId, string dbName);

        Task<CommandResult> SyncJobLevelAsync(short companyId, string dbName);
    }
}