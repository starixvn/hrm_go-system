namespace GO.Website.Controllers.Admin
{
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Core.Services;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using GO.Website.Constants;
    using GO.Website.Criteria.UserMapping;
    using GO.Website.Filters;
    using GO.Website.Models.Employee;
    using GO.Website.Models.UserMapping;
    using Microsoft.AspNetCore.Mvc;

    [Route("Admin/[controller]/[action]")]
    [GOPermission(FunctionKey = FunctionKey.UserMapping, Policy = Policy.Read)]
    public class UserMappingController(
        IUserControlSession userControlSession,
        IBaseService baseService) : BaseController
    {
        public IActionResult Index()
        {
            bool isHost = userControlSession.HasPermissionOnFunction(FunctionKey.UserMapping, Policy.Host);
            return View("~/Views/Admin/UserMapping/Index.cshtml", isHost);
        }

        public async Task<IActionResult> GetUserMapping(GetUserMappingCriteria criteria)
        {
            (var users, var subUsers) = await baseService.FetchMultipleAsync<UserMapping, UserMapping>(criteria);
            foreach (var user in users)
            {
                user.SubUsers = subUsers.Where(x => x.LinkToUserId == user.UserId).ToList();
            }

            return Json(users);
        }

        public async Task<IActionResult> SearchUser(SearchUserCriteria criteria)
        {
            var users = await baseService.GetDataAsync<BaseEmployee>(criteria);
            return Json(users);
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.UserMapping, Policy = Policy.Host)]
        public async Task<IActionResult> Create([FromBody] CreateUserMappingCommand command)
        {
            command.CreatedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.UserMapping, Policy = Policy.Host)]
        public async Task<IActionResult> Delete([FromBody] DeleteUserMappingCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }
    }
}