namespace GO.WebAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Core.Extensions.XML;
    using GO.Core.Services;
    using GO.WebAPI.Criteria.UnitHead;
    using GO.WebAPI.Models.UnitHead;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [Route("api/[controller]/[action]")]
    public class UnitHeadController(
        IHttpContextAccessor httpContextAccessor,
        IBaseService baseService) : BaseController(httpContextAccessor)
    {
        [HttpGet]
        public async Task<IActionResult> GetInfo()
        {
            var criteria = new GetUnitHeadInfoCriteria();
            var infos = await baseService.GetDataAsync<UnitHeadInfo>(criteria);

            return Json(infos);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInfo([FromBody] IList<UnitHeadInfo> infos)
        {
            var command = new UpdateUnitHeadInfoCommand
            {
                SiteId = SiteId,
                ModifiedBy = UserId,
                Value = infos != null && infos.Any() ? infos.ToXML().ToString() : string.Empty
            };
            var commandResult = await baseService.ExecuteNonQueryAsync(command);

            return Json(commandResult);
        }

        [HttpGet]
        public async Task<IActionResult> GetSchedule()
        {
            var criteria = new GetUnitHeadScheduleCriteria
            {
                FromDate = DateTime.Today,
                ToDate = DateTime.Today.AddMonths(1)
            };
            var schedules = await baseService.GetDataAsync<UnitHeadSchedule>(criteria);

            return Json(schedules);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSchedule([FromBody] IList<UnitHeadSchedule> schedules)
        {
            var command = new UpdateUnitHeadScheduleCommand
            {
                SiteId = SiteId,
                ModifiedBy = UserId,
                Value = schedules != null && schedules.Any() ? schedules.ToXML().ToString() : string.Empty
            };
            var commandResult = await baseService.ExecuteNonQueryAsync(command);

            return Json(commandResult);
        }
    }
}