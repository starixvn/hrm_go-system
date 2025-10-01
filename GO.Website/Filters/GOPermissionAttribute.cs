namespace GO.Website.Filters
{
    using System;
    using System.Linq;
    using System.Net;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class GOPermissionAttribute : Attribute, IAuthorizationFilter
    {
        public string FunctionKey { get; set; }
        public Policy Policy { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.RequestServices.GetRequiredService<IUserControlSession>();
            if (session.UserId <= 0 || session.JwtToken == null || session.JwtToken.IsExpired)
            {
                if (IsAjaxRequest(context.HttpContext.Request))
                {
                    context.Result = new UnauthorizedResult();
                }
                else
                {
                    context.Result = new RedirectToActionResult("logout", "auth", null);
                }
            }
            else
            {
                CheckUserPermission(session, context);
            }
        }

        private static void CheckUserPermission(IUserControlSession session, AuthorizationFilterContext context)
        {
            var attributes = context.ActionDescriptor.FilterDescriptors.Where(x => x.Filter is GOPermissionAttribute)
                    .Select(x => x.Filter as GOPermissionAttribute).ToList();
            var hasPermission = false;

            foreach (var attribute in attributes)
            {
                if (session.HasPermissionOnFunction(attribute.FunctionKey, attribute.Policy))
                {
                    hasPermission = true;
                }
            }

            if (!hasPermission)
            {
                if (IsAjaxRequest(context.HttpContext.Request))
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.MethodNotAllowed);
                }
                else
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
                }
            }
        }

        private static bool IsAjaxRequest(HttpRequest request)
           => string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase) ||
              string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
    }
}