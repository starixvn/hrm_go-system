namespace GO.WebAPI.Middlewares
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Net;
    using System.Threading.Tasks;
    using GO.Models;
    using GO.WebAPI.Services;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public class SiteValidationMiddleware(RequestDelegate next, ISiteService siteService)
    {
        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/Whoami"))
            {
                var jwtToken = context.Request.Headers["SiteIdentity"].ToString().Replace("Bearer ", string.Empty);
                if (string.IsNullOrWhiteSpace(jwtToken))
                {
                    await UnAuthorizedResponse(context);
                    return;
                }

                var site = DecodeJwtToken(jwtToken);
                if (site is null || site.SiteId == 0 || string.IsNullOrWhiteSpace(site.AccessToken))
                {
                    await UnAuthorizedResponse(context);
                    return;
                }

                var siteInfo = await siteService.GetByIdAsync(site.SiteId);
                if (siteInfo == null || (siteInfo.AccessToken != site.AccessToken) || !siteInfo.IsActive)
                {
                    await UnAuthorizedResponse(context);
                    return;
                }
            }

            await next(context);
        }

        private static SiteModel DecodeJwtToken(string jwt)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var siteJson = token.Payload.SerializeToJson();
                var site = JsonConvert.DeserializeObject<SiteModel>(siteJson);
                return site;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static async Task UnAuthorizedResponse(HttpContext context)
        {
            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
        }
    }
}