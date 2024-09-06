namespace GO.WebAPI.Controllers
{
    using System.Threading.Tasks;
    using GO.Core.Models;
    using GO.Core.Services;
    using GO.Models.Role;
    using GO.WebAPI.Criteria.Role;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]/[action]/{id?}")]
    public class RoleController : BaseController
    {
        private readonly IBaseService baseService;

        public RoleController(
            IHttpContextAccessor httpContextAccessor,
            IBaseService baseService) : base(httpContextAccessor)
        {
            this.baseService = baseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBySiteId(GetRoleBySiteIdCriteria criteria)
        {
            var roles = await baseService.GetDataAsync<Role>(criteria);
            return Json(roles);
        }

        [HttpGet]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var criteria = new GetRoleByUserIdCriteria
            {
                SiteId = Site.SiteId,
                UserId = userId
            };
            var roles = await baseService.GetDataAsync<Role>(criteria);
            return Json(roles);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AssignPermission([FromBody] AssignFunctionToRoleCommand command)
        {
            command.ModifiedBy = UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }
    }
}