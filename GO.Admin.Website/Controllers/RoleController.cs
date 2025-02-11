namespace GO.Admin.Website.Controllers
{
    using System.Threading.Tasks;
    using GO.Admin.Website.Constants;
    using GO.Admin.Website.Criteria.Role;
    using GO.Admin.Website.Filters;
    using GO.Admin.Website.Models;
    using GO.Admin.Website.Services;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    [UCPermission(FunctionKey = FunctionKey.RoleManagement, Policy = Policy.Read)]
    public class RoleController(
        IUserControlSession userControlSession,
        IRoleService roleService,
        IFunctionService functionService,
        IBaseService baseService) : BaseController
    {
        public IActionResult Index()
        {
            var isHost = userControlSession.HasPermissionOnFunction(FunctionKey.RoleManagement, Policy.Host);
            return View(isHost);
        }

        public async Task<IActionResult> GetBySiteId(short siteId, bool? isActive = null)
        {
            var roles = await roleService.GetBySiteIdAsync(siteId, isActive);
            return Json(roles);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.RoleManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
        {
            command.CreatedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.RoleManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Update([FromBody] UpdateRoleCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.RoleManagement, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateRoleStatusCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        public IActionResult Permission(short? siteId)
        {
            ViewBag.isHost = userControlSession.HasPermissionOnFunction(FunctionKey.RoleManagement, Policy.Host);
            return View(siteId);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.RoleManagement, Policy = Policy.Host)]
        public async Task<IActionResult> AssignPermission([FromBody] AssignPermissionToRole model)
        {
            var commandResult = await roleService.AssignPermissionAsync(model.SiteId, model.RoleId, model.Permissions);
            return Json(commandResult);
        }

        public async Task<IActionResult> GetRolePermission(short siteId, short roleId)
        {
            var functions = await functionService.GetBySiteIdAsync(siteId);
            var hierarchyFunctions = await functionService.GetHierarchyBySiteIdAsync(siteId);
            var permissions = await functionService.GetPermissionBySiteAndRoleIdAsync(siteId, roleId);
            return Json(new { functions, hierarchyFunctions, permissions });
        }
    }
}