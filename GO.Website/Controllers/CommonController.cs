namespace GO.Website.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Core.Services;
    using GO.UserControl.Client.Core;
    using GO.UserControl.Client.Core.Services;
    using GO.Website.Criteria;
    using GO.Website.Criteria.Manpower.Period;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Manpower.Period;
    using Microsoft.AspNetCore.Mvc;

    public class CommonController(
        IUserControlSettings userControlSettings,
        IBaseService baseService,
        IRoleService roleService) : BaseController
    {
        public async Task<IActionResult> SearchEmployees(SearchEmployeeCriteria criteria)
        {
            var employees = await baseService.GetDataAsync<EmployeeInfo>(criteria);

            return Json(employees);
        }

        [HttpPost]
        public async Task<IActionResult> GetManpowerPeriods([FromBody] GetPeriodCriteria criteria)
        {
            var (periods, processSettings) = await baseService.FetchMultipleAsync<ManpowerPeriod, MapnowerPeriodProcessSetting>(criteria);
            foreach (var period in periods)
            {
                period.Settings = processSettings.Where(x => x.PeriodId == period.PeriodId).ToList();
            }

            return Json(periods);
        }

        public async Task<IActionResult> GetRoles()
        {
            var roles = await roleService.GetBySiteIdAsync(userControlSettings.SiteId);
            return Json(roles);
        }
    }
}