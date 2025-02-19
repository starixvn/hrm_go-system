namespace GO.Website.Controllers
{
    using System.Threading.Tasks;
    using GO.Website.Criteria.UnitHead;
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
            var schedules = await unitHeadService.GetWeeklyScheduleAsync(criteria);

            return Json(schedules);
        }
    }
}