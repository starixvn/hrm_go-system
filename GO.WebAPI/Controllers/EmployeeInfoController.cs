namespace GO.WebAPI.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Core.Extensions.XML;
    using GO.Core.Services;
    using GO.WebAPI.Criteria.EmployeeInfo;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]/[action]")]
    public class EmployeeInfoController(IBaseService baseService) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> UpdateInfo([FromBody] UpdateEmployeeInfoCommand command)
        {
            if (command.EmployeeInfos != null && command.EmployeeInfos.Any())
            {
                command.EmployeeInfoXml = command.EmployeeInfos.ToXML().ToString();
            }
            var commandResult = await baseService.ExecuteNonQueryAsync(command);

            return Json(commandResult);
        }
    }
}