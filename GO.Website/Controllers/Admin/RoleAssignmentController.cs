namespace GO.Website.Controllers.Admin
{
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Core.Services;
    using GO.UserControl.Client.Core;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Models.User;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;
    using GO.Website.Constants;
    using GO.Website.Criteria.RoleAssignment;
    using GO.Website.Filters;
    using GO.Website.Models.RoleAssignment;
    using Microsoft.AspNetCore.Mvc;

    [Route("Admin/RoleAssignment/[action]")]
    [GOPermission(FunctionKey = FunctionKey.RoleAssignment, Policy = Policy.Read)]
    public class RoleAssignmentController(
        IUserControlSettings userControlSettings,
        IUserControlSession userControlSession,
        IBaseService baseService,
        IUserService userService) : BaseController
    {
        public IActionResult Index()
        {
            var isHost = userControlSession.HasPermissionOnFunction(FunctionKey.RoleAssignment, Policy.Host);
            return View("~/Views/Admin/RoleAssignment/Index.cshtml", isHost);
        }

        [HttpPost]
        public async Task<IActionResult> GetUserRoles([FromBody] GetUserRoleCriteria criteria)
        {
            criteria.SiteId = userControlSettings.SiteId;

            (var users, var roles) = await baseService.FetchMultipleAsync<UserInfo, UserRole>(criteria);
            foreach (var user in users)
            {
                user.Roles = roles.Where(x => x.UserId == user.UserId).ToList();
            }

            return Json(new { totalItems = criteria.TotalRows, data = users });
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.RoleAssignment, Policy = Policy.Host)]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleParam param)
        {
            var commandResult = await userService.AssignRoleAsync(param);
            return Json(commandResult);
        }
    }
}