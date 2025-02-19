namespace GO.WebAPI.Controllers
{
    using System.Threading.Tasks;
    using GO.Core.Services;
    using GO.Models.Role;
    using GO.WebAPI.Criteria.Role;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class RoleController(
        IHttpContextAccessor httpContextAccessor,
        IBaseService baseService) : BaseController(httpContextAccessor)
    {
        [HttpGet]
        [Route("api/role/getBySiteId")]
        public async Task<IActionResult> GetBySiteId(GetRoleBySiteIdCriteria criteria)
        {
            var roles = await baseService.GetDataAsync<Role>(criteria);
            return Json(roles);
        }

        [HttpGet]
        [Route("api/role/getByUserId")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var criteria = new GetRoleByUserIdCriteria
            {
                SiteId = SiteId,
                UserId = userId
            };
            var roles = await baseService.GetDataAsync<Role>(criteria);
            return Json(roles);
        }

        [HttpPost]
        [Route("api/role/assignPermission")]
        public async Task<IActionResult> AssignPermission([FromBody] AssignFunctionToRoleCommand command)
        {
            command.ModifiedBy = UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }
    }
}