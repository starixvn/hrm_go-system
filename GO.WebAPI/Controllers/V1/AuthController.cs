namespace GO.WebAPI.Controllers.V1
{
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
    using GO.WebAPI.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Route("api/v1/auth/{action}/{id?}")]
    public class AuthController : Controller
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IJwtManagerService jwtManagerService;
        private readonly IUserService userService;
        private readonly ISiteService siteService;
        private readonly ICryptoService cryptoService;
        private readonly IBaseService baseService;

        public AuthController(
            IHttpContextAccessor httpContextAccessor,
            IJwtManagerService jwtManagerService,
            IUserService userService,
            ISiteService siteService,
            ICryptoService cryptoService,
            IBaseService baseService)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.jwtManagerService = jwtManagerService;
            this.userService = userService;
            this.siteService = siteService;
            this.cryptoService = cryptoService;
            this.baseService = baseService;
        }

        [HttpPost]
        [Route("api/v1/auth/login")]
        public async Task<IActionResult> Login([FromBody] LoginParam loginParam)
        {
            string[] keyiv = loginParam.Token.Split(":");
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
            var unlinkUsers = users.Where(x => !x.LinkToUserId.HasValue).ToList();
            var linkUsers = users.Where(x => x.LinkToUserId.HasValue).ToList();

            var validUsers = GetValidUsers(unlinkUsers, loginParam.Password);
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
            var userSites = await GetValidUsersOnSiteAsync(validUsers, linkUsers, requestSite.SiteId);
            if (userSites.Count == 0)
            {
                loginResult.ErrorMessage = "User do not have permission on this site.";
                return Json(loginResult);
            }

            var user = userSites[0];
            loginResult.JwtToken = await jwtManagerService.GenerateTokenAsync(user.UserId, requestSite.SiteId);
            loginResult.IsSuccessfull = true;
            loginResult.UserId = user.UserId;

            loginResult.OnUserId = await UserCheckInAsync(user.UserId, requestSite.SiteId, loginParam);

            await LogUserLoginAsync(user.UserId, requestSite.SiteId, loginParam);

            return Json(loginResult);
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

        private SiteModel GetSiteFromRequestHeader()
        {
            string jwtToken = httpContextAccessor.HttpContext.Request.Headers["SiteIdentity"].ToString();
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken.Replace("Bearer ", string.Empty));
            var siteJson = token.Payload.SerializeToJson();
            return JsonConvert.DeserializeObject<SiteModel>(siteJson);
        }

        private IList<User> GetValidUsers(IEnumerable<User> users, string password)
        {
            return users.Where(user => user.LinkToUserId.HasValue && IsValidPassword(password, user)).ToList();
        }

        private async Task<IList<User>> GetValidUsersOnSiteAsync(IEnumerable<User> users, IEnumerable<User> linkUsers, short siteId)
        {
            var validUsers = new List<User>();
            foreach (var user in users)
            {
                var tempUsers = linkUsers.Where(x => x.LinkToUserId == user.UserId).ToList();
                if (tempUsers.Any())
                {
                    foreach (var tempUser in tempUsers)
                    {
                        await GetValidUserOnSiteAsync(validUsers, tempUser, siteId);
                    }
                }
                else
                {
                    await GetValidUserOnSiteAsync(validUsers, user, siteId);
                }
            }

            return validUsers;
        }

        private async Task GetValidUserOnSiteAsync(IList<User> validUsers, User user, short siteId)
        {
            var sites = await siteService.GetByUserIdAsync(user.UserId);
            if(sites.Any(site => site.SiteId == siteId && site.IsActive))
            {
                validUsers.Add(user);
            }
        }

        private bool IsValidPassword(string plainPwd, User user)
        {
            if (!string.IsNullOrWhiteSpace(user.BCryptPassword))
            {
                return BCrypt.Net.BCrypt.Verify(plainPwd, user.BCryptPassword);
            }

            return cryptoService.CompareStringToHash(plainPwd, user.Password);
        }
    }
}