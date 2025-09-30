namespace GO.Admin.Website.Controllers;

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GO.Admin.Website.Constants;
using GO.Admin.Website.Criteria.Company;
using GO.Admin.Website.Filters;
using GO.Admin.Website.Models;
using GO.Admin.Website.Services;
using GO.UserControl.Client.Core.Models;
using GO.UserControl.Client.Core.Models.Auth;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

public class AuthController(
    IUserControlSession session,
    IBaseService baseService,
    IUserService userService,
    ICryptoService cryptoService,
    IAuthService authService) : Controller
{
    public IActionResult Login()
    {
        var token = cryptoService.GenerateRijndaelKeyIV();
        return View("Login", token);
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginParam model)
    {
        model.IP = HttpContext.Connection?.RemoteIpAddress?.ToString();
        model.Platform = HttpContext.Request?.Headers["User-Agent"].ToString();
        model.BrowserName = HttpContext.Request?.Headers["User-Agent"].ToString();
        model.NetSessionId = HttpContext.Session.Id;

        var loginResult = await authService.LoginAsync(model);
        if (loginResult.IsSuccessfull)
        {
            session.UserId = loginResult.UserId;
            session.OnUserId = loginResult.OnUserId;
            session.JwtToken = loginResult.JwtToken;
            session.MenuItems = await userService.GetMenuItemAsync();
            session.Functions = await userService.GetFunctionPermissionAsync(session.SiteId, session.UserId);

            await DoLoginAsync(loginResult.UserId);

            return Json(new CommandResult { Code = 0, Message = string.Empty });
        }

        return Json(new CommandResult { Code = 1, Message = loginResult.ErrorMessage });
    }

    public async Task<IActionResult> Logout()
    {
        await authService.LogoutAsync(session.OnUserId);
        session.ClearAll();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [UCAuthorize]
    public IActionResult GetHeader()
    {
        var user = userService.GetById(session.UserId);
        user.UserId = 0;
        user.Password = string.Empty;

        var menus = GetMenuTrees(session.MenuItems);
        var header = new
        {
            menus,
            user
        };

        return Json(header);
    }

    private static List<MenuItem> GetMenuTrees(IList<MenuItem> menus)
    {
        var menuTrees = menus.Where(menu => !menu.ParentId.HasValue).ToList();
        foreach (var menu in menuTrees)
        {
            menu.Childs = new List<MenuItem>();
            GetChildMenus(menu, menus);
        }
        return menuTrees;
    }

    private static void GetChildMenus(MenuItem menu, IList<MenuItem> menus)
    {
        var menuItems = menus.Where(x => x.ParentId == menu.MenuItemId).ToList();
        if (menuItems.Count != 0)
        {
            foreach (var item in menuItems)
            {
                menu.Childs.Add(item);
                GetChildMenus(item, menus);
            }
        }
    }

    protected internal virtual async Task DoLoginAsync(int userId)
    {
        var companies = (await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria { IsActive = true })).ToList();
        session.Set<IList<Company>>(SessionKey.InternalCompanies, companies);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    }
}