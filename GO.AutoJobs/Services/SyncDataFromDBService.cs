namespace GO.AutoJobs.Services
{
    using System.Threading.Tasks;
    using GO.AutoJobs.Criteria.HRM;
    using GO.Core.Models;
    using GO.Core.Services;

    public class SyncDataFromDBService : ISyncDataFromDBService
    {
        private readonly IBaseService baseService;

        public SyncDataFromDBService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<CommandResult> SyncEmployeeInfoAsync(short companyId, string dbName)
        {
            var command = new ImportEmployeeFromDBCommand
            {
                CompanyId = companyId,
                DbName = dbName
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> SyncTeamAsync(short companyId, string dbName)
        {
            var command = new ImportTeamFromDBCommand
            {
                CompanyId = companyId,
                DbName = dbName
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> SyncSectionAsync(short companyId, string dbName)
        {
            var command = new ImportSectionFromDBCommand
            {
                CompanyId = companyId,
                DbName = dbName
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> SyncDepartmentAsync(short companyId, string dbName)
        {
            var command = new ImportDepartmentFromDBCommand
            {
                CompanyId = companyId,
                DbName = dbName
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> SyncUnitAsync(short companyId, string dbName)
        {
            var command = new ImportUnitFromDBCommand
            {
                CompanyId = companyId,
                DbName = dbName
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> SyncJobTitleAsync(short companyId, string dbName)
        {
            var command = new ImportJobTitleFromDBCommand
            {
                CompanyId = companyId,
                DbName = dbName
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> SyncJobLevelAsync(short companyId, string dbName)
        {
            var command = new ImportJobLevelFromDBCommand
            {
                CompanyId = companyId,
                DbName = dbName
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }
    }
}