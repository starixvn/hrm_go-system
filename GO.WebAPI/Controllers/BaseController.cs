namespace GO.WebAPI.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using GO.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class BaseController : Controller
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public BaseController(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            GetIdentityFromClaims();
            GetSiteFromRequestHeader();
        }

        public int UserId { get; private set; }

        public SiteModel Site { get; private set; }

        private void GetIdentityFromClaims()
        {
            if (httpContextAccessor.HttpContext.User is ClaimsPrincipal identity)
            {
                UserId = int.Parse(identity.FindFirst("UserId").Value);
            }
        }

        private void GetSiteFromRequestHeader()
        {
            string jwtToken = httpContextAccessor.HttpContext.Request.Headers["SiteIdentity"].ToString();
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken.Replace("Bearer ", string.Empty));
            var siteJson = token.Payload.SerializeToJson();
            Site = JsonConvert.DeserializeObject<SiteModel>(siteJson);
        }
    }
}