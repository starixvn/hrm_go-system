namespace GO.Website.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.Data;
    using GO.Core.Services;
    using GO.UserControl.Client.Core;
    using GO.UserControl.Client.Core.Models;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;
    using GO.Website.Criteria;
    using GO.Website.Models;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController(
        IUserControlSession userControlSession,
        IAuthService authService,
        IUserService userService,
        IBaseService baseService,
        OtpConfig otpConfig) : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetSites()
        {
            var userId = HttpUtility.UrlEncode(Cryptography.EncryptPassword(userControlSession.UserId.ToString()));
            var onUserId = HttpUtility.UrlEncode(Cryptography.EncryptPassword(userControlSession.OnUserId.ToString()));
            var criteria = new GetUserSiteByUserIdCriteria
            {
                UserId = userControlSession.UserId
            };
            var usersites = await baseService.GetDataAsync<UserSite>(criteria);
            usersites = usersites.Where(x => x.SiteId != UserControlClient.SiteId).ToList();
            foreach (var usersite in usersites)
            {
                var username = !string.IsNullOrWhiteSpace(usersite.Username) ? usersite.Username : usersite.Code;
                username = !string.IsNullOrWhiteSpace(username) ? HttpUtility.UrlEncode(Cryptography.EncryptPassword(username)) : string.Empty;
                var siteUrl = !string.IsNullOrWhiteSpace(usersite.SSOUrl) ? usersite.SSOUrl : usersite.SiteURL;
                usersite.SiteURL = $"{siteUrl}?userId={userId}&onUserId={onUserId}&username={username}";
            }

            return Json(usersites);
        }

        [Route("sso/gettime")]
        public async Task<IActionResult> GetTime()
        {
            var time = HttpUtility.UrlEncode(Cryptography.EncryptPassword(await authService.SSOGetTimeAsync()));
            return Json(time);
        }

        public IActionResult GetUserInfo()
        {
            var user = userService.GetById(userControlSession.UserId);
            user.UserId = 0;
            user.Password = string.Empty;
            return Json(user);
        }

        public IActionResult GetOtpConfig()
        {
            return Json(otpConfig);
        }

        public IActionResult GetMenus()
        {
            var menus = GetMenuTrees(userControlSession.MenuItems);
            return Json(menus);
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
            if (menuItems.Any())
            {
                foreach (var item in menuItems)
                {
                    menu.Childs.Add(item);
                    GetChildMenus(item, menus);
                }
            }
        }
    }
}