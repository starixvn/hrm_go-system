namespace GO.WebAPI.Controllers
{
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
    [Route("api/[controller]/[action]/{id?}")]
    public class UserController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly IUserService userService;

        public UserController(
            IHttpContextAccessor httpContextAccessor,
            IBaseService baseService,
            ICryptoService cryptoService,
            IUserService userService) : base(httpContextAccessor)
        {
            this.baseService = baseService;
            this.userService = userService;
            this.cryptoService = cryptoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuItem()
        {
            var criteria = new GetMenuItemByUserIdCriteria
            {
                UserId = UserId,
                SiteId = Site.SiteId
            };
            var menuItems = await baseService.GetDataAsync<MenuItem>(criteria);
            return Json(menuItems);
        }

        [HttpGet]
        public async Task<IActionResult> GetFunctionPermission(GetFunctionByUserIdCriteria criteria)
        {
            var functions = await baseService.GetDataAsync<FunctionPermission>(criteria);
            return Json(functions);
        }

        [HttpGet]
        public async Task<IActionResult> GetBySiteId(GetUserBySiteIdCriteria criteria)
        {
            criteria.SiteId = Site.SiteId;
            var users = await baseService.GetDataAsync<User>(criteria);
            return Json(users);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            User user = await userService.GetByIdAsync(id);
            return Json(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            command.SiteId = Site.SiteId;
            command.CreatedBy = UserId;
            if (!string.IsNullOrWhiteSpace(command.BCryptPassword))
            {
                string[] keyiv = command.Token.Split(":");
                var rijndaelKey = keyiv[0];
                var rijndaelIV = keyiv[1];
                command.BCryptPassword = cryptoService.Decrypt(command.BCryptPassword, rijndaelKey, rijndaelIV);
                command.BCryptPassword = BCrypt.Net.BCrypt.HashPassword(command.BCryptPassword);
            }

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            commandResult.ObjectId = command.UserId;
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdateUserInfoCommand command)
        {
            command.SiteId = Site.SiteId;
            command.ModifiedBy = UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleToUserCommand command)
        {
            command.SiteId = Site.SiteId;
            command.ModifiedBy = UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordParam param)
        {
            string[] keyiv = param.Token.Split(":");
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
                    SiteId = Site.SiteId,
                    BCryptPassword = BCrypt.Net.BCrypt.HashPassword(param.Password),
                    IsResetPassword = false,
                    ModifiedBy = UserId
                };
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] UpdatePasswordParam param)
        {
            string[] keyiv = param.Token.Split(":");
            var rijndaelKey = keyiv[0];
            var rijndaelIV = keyiv[1];
            param.Password = cryptoService.Decrypt(param.Password, rijndaelKey, rijndaelIV);
            var command = new UpdateUserPasswordCommand
            {
                UserId = param.UserId,
                SiteId = Site.SiteId,
                BCryptPassword = BCrypt.Net.BCrypt.HashPassword(param.Password),
                IsResetPassword = true,
                ModifiedBy = UserId
            };
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);

            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] DeleteUserCommand command)
        {
            command.SiteId = Site.SiteId;
            command.ModifiedBy = UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
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