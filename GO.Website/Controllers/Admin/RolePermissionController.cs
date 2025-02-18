namespace GO.Website.Controllers.Admin
{
    using System.Threading.Tasks;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Models;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;
    using GO.Website.Constants;
    using GO.Website.Filters;
    using GO.Website.Models.Admin.RolePermission;
    using Microsoft.AspNetCore.Mvc;

    [Route("Admin/[controller]/[action]")]
    [GOPermission(FunctionKey = FunctionKey.AdminPageRolePermission, Policy = Policy.Read)]
    public class RolePermissionController(
        IUserControlSession userControlSession,
        IRoleService roleService,
        IFunctionService functionService) : BaseController
    {
        public IActionResult Index()
        {
            var isHost = userControlSession.HasPermissionOnFunction(FunctionKey.AdminPageRolePermission, Policy.Host);
            return View("~/Views/Admin/RolePermission/Index.cshtml", isHost);
        }

        public async Task<IActionResult> GetRolePermission(short roleId)
        {
            var functions = await functionService.GetBySiteIdAsync(userControlSession.SiteId);
            var hierarchyFunctions = await functionService.GetHierarchyBySiteIdAsync(userControlSession.SiteId);
            var permissions = await functionService.GetPermissionBySiteAndRoleIdAsync(userControlSession.SiteId, roleId);

            return Json(new { functions, permissions, hierarchyFunctions });
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.AdminPageRolePermission, Policy = Policy.Host)]
        public async Task<IActionResult> AssignPermission([FromBody] AssignPermissionToRole model)
        {
            CommandResult commandResult = await roleService.AssignPermissionAsync(userControlSession.SiteId, model.RoleId, model.Permissions);
            return Json(commandResult);
        }
    }
}