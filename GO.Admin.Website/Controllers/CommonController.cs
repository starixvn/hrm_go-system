namespace GO.Admin.Website.Controllers
{
    using System.Threading.Tasks;
    using GO.Admin.Website.Criteria.Site;
    using GO.Admin.Website.Models.Site;
    using GO.Admin.Website.Services;
    using GO.UserControl.Client.Core.Services;
    using Microsoft.AspNetCore.Mvc;

    public class CommonController(
        IBaseService baseService,
        IRoleService roleService) : BaseController
    {
        public async Task<IActionResult> GetAllSite(GetAllSiteCriteria criteria)
        {
            var sites = await baseService.GetDataAsync<Site>(criteria);
            return Json(sites);
        }

        public async Task<IActionResult> GetRoleBySiteId(short siteId)
        {
            var roles = await roleService.GetBySiteIdAsync(siteId);
            return Json(roles);
        }
    }
}