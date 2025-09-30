namespace GO.Admin.Website.Controllers;

using System.Linq;
using System.Threading.Tasks;
using GO.Admin.Website.Constants;
using GO.Admin.Website.Criteria.User;
using GO.Admin.Website.Filters;
using GO.Admin.Website.Models.Site;
using GO.Admin.Website.Models.User;
using GO.Admin.Website.Services;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Models.User;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using Microsoft.AspNetCore.Mvc;

[UCPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Read)]
public class UserController(
    IUserControlSession session,
    IBaseService baseService,
    ICryptoService cryptoService,
    IFunctionService functionService,
    IUserService userService) : BaseController
{
    public IActionResult Index()
    {
        var token = cryptoService.GenerateRijndaelKeyIV();
        ViewBag.isHost = session.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host);
        return View("Index", token);
    }

    [HttpPost]
    public async Task<IActionResult> GetAll([FromBody] GetAllUserCriteria criteria)
    {
        var users = await baseService.GetDataAsync<Models.User.User>(criteria);
        return Json(new { totalItems = criteria.TotalRows, data = users });
    }

    [HttpPost]
    [UCPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
    public async Task<IActionResult> Create([FromBody] CreateUserParam param)
    {
        var commandResult = await userService.CreateAsync(param);
        return Json(commandResult);
    }

    [HttpPost]
    [UCPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
    public async Task<IActionResult> ResetPassword([FromBody] UpdatePasswordParam param)
    {
        var commandResult = await userService.ResetPasswordAsync(param);
        return Json(commandResult);
    }

    public async Task<IActionResult> AssignSite(int id)
    {
        var user = await userService.GetByIdAsync(id);
        if (user == null)
        {
            return RedirectToAction("PageNotFound", "Error");
        }

        user.Password = string.Empty;
        ViewBag.IsHost = session.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host);
        return View(user);
    }

    [HttpPost]
    [UCPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
    public async Task<IActionResult> AssignSite([FromBody] AssignSiteToUserCommand command)
    {
        command.CreatedBy = session.UserId;
        var commandResult = await baseService.ExecuteNonQueryAsync(command);
        return Json(commandResult);
    }

    public async Task<IActionResult> GetSiteByUserId(int id)
    {
        var criteria = new GetSiteByUserIdCriteria { UserId = id };
        var (sites, roles) = await baseService.FetchMultipleAsync<UserSite, UserRole>(criteria);
        foreach (var site in sites)
        {
            site.Roles = roles.Where(x => x.SiteId == site.SiteId).ToList();
        }

        return Json(sites);
    }

    public async Task<IActionResult> GetAvailableSiteByUserId(int id)
    {
        var criteria = new GetAvailableSiteByUserIdCriteria { UserId = id };
        var sites = await baseService.GetDataAsync<Site>(criteria);
        return Json(sites);
    }

    [HttpPost]
    [UCPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
    public async Task<IActionResult> UpdateUserSiteStatus([FromBody] UpdateUserSiteStatusCommand command)
    {
        command.ModifiedBy = session.UserId;
        var commandResult = await baseService.ExecuteNonQueryAsync(command);
        return Json(commandResult);
    }

    [HttpPost]
    [UCPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
    public async Task<IActionResult> UpdateUserSiteRole([FromBody] UpdateUserSiteRoleCommand command)
    {
        command.ModifiedBy = session.UserId;
        var commandResult = await baseService.ExecuteNonQueryAsync(command);
        return Json(commandResult);
    }

    public async Task<IActionResult> Permission(int id)
    {
        var user = await userService.GetByIdAsync(id);
        if (user == null)
        {
            return RedirectToAction("PageNotFound", "Error");
        }

        user.Password = string.Empty;
        return View(user);
    }

    public async Task<IActionResult> GetUserPermission(short siteId, int userId)
    {
        var functions = await functionService.GetHierarchyBySiteIdAsync(siteId, true);
        var permissions = await userService.GetFunctionPermissionAsync(siteId, userId);
        return Json(new { functions, permissions });
    }

    public async Task<IActionResult> UserMapping(int id)
    {
        var user = await userService.GetByIdAsync(id);
        if (user == null)
        {
            return RedirectToAction("PageNotFound", "Error");
        }

        user.Password = string.Empty;
        ViewBag.IsHost = session.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host);
        return View("UserMapping", user);
    }

    public async Task<IActionResult> GetUserMappingByUserId(int id)
    {
        var criteria = new GetMappingByUserIdCriteria { UserId = id };
        var mappings = await baseService.GetDataAsync<UserMapping>(criteria);
        mappings = mappings.Where(x => x.SiteId != session.SiteId).ToList();

        return Json(mappings);
    }

    [HttpPost]
    public async Task<IActionResult> SearchUserMapping([FromBody] SearchUserMappingCriteria criteria)
    {
        var users = await baseService.GetDataAsync<Models.User.User>(criteria);
        return Json(users);
    }

    [HttpPost]
    [UCPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
    public async Task<IActionResult> UpdateUserMapping([FromBody] UpdateUserMappingCommand command)
    {
        command.ModifiedBy = session.UserId;
        var commandResult = await baseService.ExecuteNonQueryAsync(command);
        return Json(commandResult);
    }
}