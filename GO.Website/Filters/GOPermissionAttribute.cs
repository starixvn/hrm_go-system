namespace GO.Website.Filters
{
    using System;
    using System.Linq;
    using HRM.UserControl.Client.Core;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class GOPermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IUserControlSession session;

        public GOPermissionAttribute() : this(null)
        {
        }

        internal GOPermissionAttribute(IUserControlSession session)
        {
            this.session = session ?? UserControlClientManager.UserControlSession;
        }

        public string FunctionKey { get; set; }
        public Policy Policy { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
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
                CheckUserPermission(context);
            }
        }

        private void CheckUserPermission(AuthorizationFilterContext context)
        {
            var functionKeys = FunctionKey.Split(',').Where(x => session.HasPermissionOnFunction(x, Policy));
            var hasPermission = functionKeys.Any();

            if (!hasPermission)
            {
                if (IsAjaxRequest(context.HttpContext.Request))
                {
                    context.Result = new ForbidResult();
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