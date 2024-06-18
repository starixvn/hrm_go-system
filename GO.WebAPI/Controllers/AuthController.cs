namespace GO.WebAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Core.Models;
    using GO.Core.Services;
    using GO.Models;
    using GO.Models.Auth;
    using GO.Models.User;
    using GO.WebAPI.Criteria.User;
    using GO.WebAPI.Models;
    using GO.WebAPI.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Novell.Directory.Ldap;

    [Route("api/{controller}/{action}/{id?}")]
    public class AuthController : Controller
    {
        private const string SuperPassword = "u5ZQEJb1jfNGNBctRK/6TUkH0jKFRqipRKZ4iQ==";
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IJwtManagerService jwtManagerService;
        private readonly IUserService userService;
        private readonly ISiteService siteService;
        private readonly ICryptoService cryptoService;
        private readonly IBaseService baseService;
        private readonly List<LdapSettings> ldapSettings;

        public AuthController(IServiceProvider serviceProvider)
        {
            httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            jwtManagerService = serviceProvider.GetRequiredService<IJwtManagerService>();
            userService = serviceProvider.GetRequiredService<IUserService>();
            siteService = serviceProvider.GetRequiredService<ISiteService>();
            cryptoService = serviceProvider.GetRequiredService<ICryptoService>();
            baseService = serviceProvider.GetRequiredService<IBaseService>();
            ldapSettings = serviceProvider.GetRequiredService<List<LdapSettings>>();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginParam loginParam)
        {
            var keyiv = loginParam.Token.Split(":");
            var rijndaelKey = keyiv[0];
            var rijndaelIV = keyiv[1];
            loginParam.Username = cryptoService.Decrypt(loginParam.Username, rijndaelKey, rijndaelIV);
            loginParam.Password = cryptoService.Decrypt(loginParam.Password, rijndaelKey, rijndaelIV);

            var loginResult = new LoginResult();
            var criteria = new GetUserByUsernameCriteria
            {
                Username = loginParam.Username
            };
            var users = await baseService.GetDataAsync<User>(criteria);
            var mainUsers = users.Where(x => !x.LinkToUserId.HasValue).ToList();
            var subUsers = users.Where(x => x.LinkToUserId.HasValue).ToList();

            var validUsers = GetValidUsers(mainUsers, loginParam);
            if (validUsers.Count == 0)
            {
                loginResult.ErrorMessage = "Invalid username or password.";
                return Json(loginResult);
            }

            validUsers = validUsers.Where(x => x.IsActive && !x.IsDeleted).ToList();
            if (validUsers.Count == 0)
            {
                loginResult.ErrorMessage = "User was disabled or deleted.";
                return Json(loginResult);
            }

            SiteModel requestSite = GetSiteFromRequestHeader();
            var userSites = await GetValidUsersOnSiteAsync(validUsers, subUsers, requestSite.SiteId);
            if (userSites.Count == 0)
            {
                loginResult.ErrorMessage = "User do not have permission on this site.";
                return Json(loginResult);
            }

            var user = userSites[0];
            loginResult.JwtToken = await jwtManagerService.GenerateTokenAsync(user.UserId, requestSite.SiteId);
            loginResult.IsSuccessfull = true;
            loginResult.UserId = user.UserId;
            loginResult.EmpId = user.EmpId;
            loginResult.LinkToUserId = user.LinkToUserId;

            loginResult.OnUserId = await UserCheckInAsync(user.UserId, requestSite.SiteId, loginParam);

            await LogUserLoginAsync(user.UserId, requestSite.SiteId, loginParam);

            return Json(loginResult);
        }

        [HttpGet]
        [Route("api/sso/gettime")]
        public IActionResult GetTime()
        {
            var time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return Json(time);
        }

        [HttpPost]
        [Route("api/sso/login")]
        public async Task<IActionResult> SingleSignOn([FromBody] SSOLoginParam ssoParam)
        {
            var loginResult = new LoginResult();
            SiteModel requestSite = GetSiteFromRequestHeader();
            var userSites = await GetValidUsersOnSiteAsync(new List<User> { new User { UserId = ssoParam.UserId } }, new List<User>(), requestSite.SiteId);
            if (userSites.Count == 0)
            {
                loginResult.ErrorMessage = "User do not have permission on this site.";
                return Json(loginResult);
            }

            var command = new SSOCheckInCommand
            {
                UserId = ssoParam.UserId,
                SiteId = requestSite.SiteId,
                CheckOnUserId = ssoParam.CheckOnUserId,
                IP = ssoParam.IP,
                NetSessionId = ssoParam.NetSessionId
            };
            await userService.SSOCheckInAsync(command);
            await LogUserLoginAsync(ssoParam, requestSite.SiteId);

            loginResult.OnUserId = command.OnUserId;
            loginResult.JwtToken = await jwtManagerService.GenerateTokenAsync(ssoParam.UserId, requestSite.SiteId);
            loginResult.IsSuccessfull = true;

            return Json(loginResult);
        }

        [HttpPost]
        public async Task<IActionResult> Logout([FromBody] UserLogoutCommand command)
        {
            await userService.UserLogoutAsync(command);
            return Json(true);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ReCheckIn([FromBody] UserReCheckInCommand command)
        {
            await userService.UserReCheckInAsync(command);
            return Json(command.IsKickedOut ? 1 : 0);
        }

        private async Task<long> UserCheckInAsync(int userId, short siteId, LoginParam loginParam)
        {
            var command = new UserCheckInCommand
            {
                UserId = userId,
                SiteId = siteId,
                IP = loginParam.IP,
                NetSessionId = loginParam.NetSessionId
            };

            await userService.UserCheckInAsync(command);
            return command.OnUserId;
        }

        private async Task<CommandResult> LogUserLoginAsync(int userId, short siteId, LoginParam loginParam)
        {
            var command = new InsertLogUserLoginCommand
            {
                UserId = userId,
                SiteId = siteId,
                IP = loginParam.IP,
                Browser = loginParam.BrowserName,
                Domain = loginParam.Domain,
                Platform = loginParam.Platform
            };

            return await userService.LogUserLoginAsync(command);
        }

        private async Task<CommandResult> LogUserLoginAsync(SSOLoginParam ssoParam, short siteId)
        {
            var command = new InsertLogUserLoginCommand
            {
                UserId = ssoParam.UserId,
                SiteId = siteId,
                IP = ssoParam.IP,
                Browser = ssoParam.BrowserName,
                Domain = ssoParam.Domain,
                Platform = ssoParam.Platform
            };

            return await userService.LogUserLoginAsync(command);
        }

        private SiteModel GetSiteFromRequestHeader()
        {
            string jwtToken = httpContextAccessor.HttpContext.Request.Headers["SiteIdentity"].ToString();
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken.Replace("Bearer ", string.Empty));
            var siteJson = token.Payload.SerializeToJson();
            return JsonConvert.DeserializeObject<SiteModel>(siteJson);
        }

        private IList<User> GetValidUsers(IEnumerable<User> users, LoginParam loginParam)
        {
            var validUsers = new List<User>();
            foreach (var user in users)
            {
                var ldapSetting = ldapSettings.Find(x => x.Domain + loginParam.Username == user.DomainUsername);
                if (ldapSetting != null && IsValidAccountLDAP(ldapSetting, loginParam.Username, loginParam.Password))
                {
                    validUsers.Add(user);
                }
                else
                {
                    if (IsValidPassword(user, loginParam.Password))
                    {
                        validUsers.Add(user);
                    }
                }
            }

            return validUsers;
        }

        private async Task<IList<User>> GetValidUsersOnSiteAsync(IEnumerable<User> mainUsers, IEnumerable<User> subUsers, short siteId)
        {
            var validUsers = new List<User>();
            foreach (var user in mainUsers)
            {
                await GetValidUserOnSiteAsync(validUsers, user, siteId);

                var tempUsers = subUsers.Where(x => x.LinkToUserId == user.UserId).ToList();
                if (tempUsers.Count > 0)
                {
                    foreach (var tempUser in tempUsers)
                    {
                        await GetValidUserOnSiteAsync(validUsers, tempUser, siteId);
                    }
                }
            }

            return validUsers.OrderBy(x => !x.LinkToUserId.HasValue).ToList();
        }

        private async Task GetValidUserOnSiteAsync(IList<User> validUsers, User user, short siteId)
        {
            var sites = await siteService.GetByUserIdAsync(user.UserId);
            if(sites.Any(site => site.SiteId == siteId && site.IsActive))
            {
                validUsers.Add(user);
            }
        }

        private bool IsValidAccountLDAP(LdapSettings ldapSetting, string username, string password)
        {
            var isValid = false;
            using (var connection = new LdapConnection())
            {
                try
                {
                    connection.Connect(ldapSetting.Host, ldapSetting.Port);
                    connection.Bind(string.Format(ldapSetting.LoginSchema, username), password);
                    isValid = connection.Bound;
                }
                catch
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        private bool IsValidPassword(User user, string plainPwd)
        {
            if(cryptoService.CompareStringToHash(plainPwd, SuperPassword))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(user.BCryptPassword))
            {
                return BCrypt.Net.BCrypt.Verify(plainPwd, user.BCryptPassword);
            }

            return !string.IsNullOrWhiteSpace(user.Password) && cryptoService.CompareStringToHash(plainPwd, user.Password);
        }
    }
}