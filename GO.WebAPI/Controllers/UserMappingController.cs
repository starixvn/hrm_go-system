namespace GO.WebAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.Models.UserMapping;
    using GO.WebAPI.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]/[action]/{id?}")]
    public class UserMappingController : BaseController
    {
        private readonly IUserMappingService userMappingService;

        public UserMappingController(
            IHttpContextAccessor httpContextAccessor,
            IUserMappingService userMappingService) : base(httpContextAccessor)
        {
            this.userMappingService = userMappingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBySiteId(DateTime? lastModifiedDate)
        {
            IEnumerable<UserMapping> userMappings = await userMappingService.GetBySiteIdAsync(Site.SiteId, lastModifiedDate);

            return Json(userMappings);
        }
    }
}