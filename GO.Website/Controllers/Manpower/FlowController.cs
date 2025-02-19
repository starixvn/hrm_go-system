namespace GO.Website.Controllers.Manpower
{
    using System.Linq;
    using System.Threading.Tasks;
    using Constants;
    using Filters;
    using GO.Core.Extensions.XML;
    using GO.Core.Services;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using GO.Website.Criteria.Manpower;
    using GO.Website.Models.Manpower;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [Route("Manpower/Flow/[action]")]
    [GOPermission(FunctionKey = FunctionKey.ManpowerForecast_FlowManagement, Policy = Policy.Read)]
    public class FlowController(
        IUserControlSession userControlSession,
        IBaseService baseService) : BaseController
    {
        public IActionResult Index()
        {
            var isHost = userControlSession.HasPermissionOnFunction(FunctionKey.ManpowerForecast_FlowManagement, Policy.Host);
            return View("~/Views/Manpower/Flow/Index.cshtml", isHost);
        }

        public async Task<IActionResult> GetUnits()
        {
            var criteria = new GetUnitCriteria();
            var units = await baseService.GetDataAsync<Unit>(criteria);

            return Json(units);
        }

        public async Task<IActionResult> GetFlows()
        {
            var criteria = new GetFlowCriteria();
            var flows = await baseService.GetDataAsync<ManpowerFlow>(criteria);

            return Json(flows);
        }

        public async Task<IActionResult> GetByUnit(GetFlowByUnitCriteria criteria)
        {
            var flows = await baseService.GetDataAsync<ManpowerFlow>(criteria);

            return Json(flows);
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.ManpowerForecast_FlowManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Update([FromBody] UpdateFlowParam param)
        {
            var command = new UpdateFlowCommand
            {
                UnitName = param.UnitName,
                FlowXml = param.Flows != null && param.Flows.Any() ? param.Flows.ToXML().ToString() : string.Empty,
                ModifiedBy = userControlSession.UserId
            };
            var commandResult = await baseService.ExecuteNonQueryAsync(command);

            return Json(commandResult);
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.ManpowerForecast_FlowManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Delete([FromBody] DeleteFlowCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);

            return Json(commandResult);
        }
    }
}