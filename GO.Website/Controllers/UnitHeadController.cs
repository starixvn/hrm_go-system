namespace GO.Website.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.Website.Criteria.UnitHead;
    using GO.Website.Models.UnitHead;
    using GO.Website.Services;
    using Microsoft.AspNetCore.Mvc;

    public class UnitHeadController(IUnitHeadService unitHeadService) : Controller
    {
        public IActionResult Schedule()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetWeeklySchedule([FromBody] GetUnitHeadWeeklyScheduleCriteria criteria)
        {
            IEnumerable<UnitHeadWeeklySchedule> schedules = await unitHeadService.GetWeeklyScheduleAsync(criteria);

            return Json(schedules);
        }
    }
}