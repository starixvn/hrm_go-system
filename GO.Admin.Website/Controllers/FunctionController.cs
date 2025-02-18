namespace GO.Admin.Website.Controllers
{
    using System.Threading.Tasks;
    using GO.Admin.Website.Constants;
    using GO.Admin.Website.Criteria.Function;
    using GO.Admin.Website.Filters;
    using GO.Admin.Website.Services;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    [UCPermission(FunctionKey = FunctionKey.FunctionManagement, Policy = Policy.Read)]
    public class FunctionController(
        IBaseService baseService,
        IFunctionService functionService,
        IUserControlSession userControlSession) : BaseController
    {
        public IActionResult Index()
        {
            var isHost = userControlSession.HasPermissionOnFunction(FunctionKey.FunctionManagement, Policy.Host);
            return View(isHost);
        }

        public async Task<IActionResult> GetBySiteId(short id)
        {
            var functions = await functionService.GetHierarchyBySiteIdAsync(id, null);
            return Json(functions);
        }

        public async Task<IActionResult> GetParentBySiteId(short id)
        {
            var functions = await functionService.GetBySiteIdAsync(id, null);
            return Json(functions);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.FunctionManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Create([FromBody] CreateFunctionCommand command)
        {
            command.CreatedBy = userControlSession.UserId;
            command.FunctionKey = command.FunctionName.Trim().Replace(" ", "");
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.FunctionManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Update([FromBody] EditFunctionCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.FunctionManagement, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateFunctionStatusCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }
    }
}