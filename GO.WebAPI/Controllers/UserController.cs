namespace GO.WebAPI.Controllers;

using System.Threading.Tasks;
using GO.Core.Models;
using GO.Core.Services;
using GO.Models;
using GO.Models.User;
using GO.WebAPI.Criteria.Function;
using GO.WebAPI.Criteria.MenuItem;
using GO.WebAPI.Criteria.User;
using GO.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("api/user")]
public class UserController(
    IHttpContextAccessor httpContextAccessor,
    IBaseService baseService,
    ICryptoService cryptoService,
    IUserService userService) : BaseController(httpContextAccessor)
{
    [HttpGet]
    [Route("getMenuItem")]
    public async Task<IActionResult> GetMenuItem()
    {
        var criteria = new GetMenuItemByUserIdCriteria
        {
            UserId = UserId,
            SiteId = SiteId
        };
        var menuItems = await baseService.GetDataAsync<MenuItem>(criteria);
        return Json(menuItems);
    }

    [HttpGet]
    [Route("getFunctionPermission")]
    public async Task<IActionResult> GetFunctionPermission(GetFunctionByUserIdCriteria criteria)
    {
        var functions = await baseService.GetDataAsync<FunctionPermission>(criteria);
        return Json(functions);
    }

    [HttpGet]
    [Route("getAll")]
    public async Task<IActionResult> GetBySiteId(GetUserBySiteIdCriteria criteria)
    {
        criteria.SiteId = criteria.SiteId > 0 ? criteria.SiteId : SiteId;
        var users = await baseService.GetDataAsync<User>(criteria);
        return Json(users);
    }

    [HttpGet]
    [Route("getById/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await userService.GetByIdAsync(id);
        return Json(user);
    }

    [HttpGet]
    [Route("getSites/{id:int}")]
    public async Task<IActionResult> GetSiteAccess(int id)
    {
        var sites = await userService.GetSiteAccessAsync(id);
        return Json(sites);
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        command.SiteId = SiteId;
        command.CreatedBy = UserId;
        if (!string.IsNullOrWhiteSpace(command.BCryptPassword))
        {
            var keyiv = command.Token.Split(":");
            var rijndaelKey = keyiv[0];
            var rijndaelIV = keyiv[1];
            command.BCryptPassword = cryptoService.Decrypt(command.BCryptPassword, rijndaelKey, rijndaelIV);
            command.BCryptPassword = BCrypt.Net.BCrypt.HashPassword(command.BCryptPassword);
        }

        var commandResult = await baseService.ExecuteNonQueryAsync(command);
        commandResult.ObjectId = command.UserId;
        return Json(commandResult);
    }

    [HttpPost]
    [Route("update")]
    public async Task<IActionResult> Update([FromBody] UpdateUserInfoCommand command)
    {
        command.SiteId = SiteId;
        command.ModifiedBy = UserId;
        var commandResult = await baseService.ExecuteNonQueryAsync(command);
        return Json(commandResult);
    }

    [HttpPost]
    [Route("assignRole")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleToUserCommand command)
    {
        command.SiteId = command.SiteId > 0 ? command.SiteId : SiteId;
        command.ModifiedBy = UserId;
        var commandResult = await baseService.ExecuteNonQueryAsync(command);
        return Json(commandResult);
    }

    [HttpPost]
    [Route("updatePassword")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordParam param)
    {
        var keyiv = param.Token.Split(":");
        var rijndaelKey = keyiv[0];
        var rijndaelIV = keyiv[1];
        param.OldPassword = cryptoService.Decrypt(param.OldPassword, rijndaelKey, rijndaelIV);
        param.Password = cryptoService.Decrypt(param.Password, rijndaelKey, rijndaelIV);
        var commandResult = new CommandResult();
        var user = await userService.GetByIdAsync(UserId);
        if (!IsValidPassword(param.OldPassword, user))
        {
            commandResult.Message = "Wrong password.";
        }
        else
        {
            var command = new UpdateUserPasswordCommand
            {
                UserId = UserId,
                SiteId = SiteId,
                Password = BCrypt.Net.BCrypt.HashPassword(param.Password),
                IsResetPassword = false,
                ModifiedBy = UserId
            };
            commandResult = await userService.UpdatePasswordAsync(command);
        }

        return Json(commandResult);
    }

    [HttpPost]
    [Route("resetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] UpdatePasswordParam param)
    {
        var keyiv = param.Token.Split(":");
        var rijndaelKey = keyiv[0];
        var rijndaelIV = keyiv[1];
        param.Password = cryptoService.Decrypt(param.Password, rijndaelKey, rijndaelIV);
        var command = new UpdateUserPasswordCommand
        {
            UserId = param.UserId,
            SiteId = SiteId,
            Password = BCrypt.Net.BCrypt.HashPassword(param.Password),
            IsResetPassword = true,
            ModifiedBy = UserId
        };
        var commandResult = await baseService.ExecuteNonQueryAsync(command);

        return Json(commandResult);
    }

    [HttpPost]
    [Route("delete")]
    public async Task<IActionResult> Delete([FromBody] DeleteUserCommand command)
    {
        command.SiteId = SiteId;
        command.ModifiedBy = UserId;
        var commandResult = await baseService.ExecuteNonQueryAsync(command);
        return Json(commandResult);
    }

    [HttpPost]
    [Route("updateUserSite")]
    public async Task<IActionResult> UpdateUserSite([FromBody] UpdateUserSiteCommand command)
    {
        command.ModifiedBy = UserId;
        var commandResult = await baseService.ExecuteNonQueryAsync(command);
        return Json(commandResult);
    }

    private bool IsValidPassword(string plainPwd, User user)
    {
        if (!string.IsNullOrWhiteSpace(user.Password))
        {
            return BCrypt.Net.BCrypt.Verify(plainPwd, user.Password);
        }

        return cryptoService.CompareStringToHash(plainPwd, user.OldPassword);
    }
}