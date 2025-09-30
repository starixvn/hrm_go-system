using System;
using System.Threading;
using System.Threading.Tasks;
using Fanex.Data.SqlServer;
using Fanex.Logging.Sentry.RavenClient;
using GO.Admin.Website;
using GO.Admin.Website.Middlewares;
using GO.UserControl.Client.Core;
using GO.UserControl.Client.Core.Session;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddHttpContextAccessor();
services.AddFanexSql(configuration);
services.AddFanexLogSentry(configuration);

services.AddMemoryCache();

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = configuration.GetSection("AuthSettings:LoginPath").Value;
        options.LogoutPath = configuration.GetSection("AuthSettings:LogoutPath").Value;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(configuration.GetSection("AuthSettings:SessionTimeout").Value));
        options.SlidingExpiration = true;
        options.Cookie = new CookieBuilder
        {
            Name = ".UserControl.Admin.Cookies",
            SecurePolicy = CookieSecurePolicy.SameAsRequest
        };
        options.Events.OnSigningOut = context =>
        {
            context.Response.Redirect(options.LoginPath);
            return Task.CompletedTask;
        };
    });

services.AddSession(options =>
{
    options.Cookie.Name = ".UserControl.Admin.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(int.Parse(configuration.GetSection("AuthSettings:SessionTimeout").Value));
    options.Cookie.IsEssential = true;
});

services.AddControllersWithViews();

services.RegisterDI();

var app = builder.Build();

UserControlClient
    .UseSession(app.Services.GetRequiredService<IUserControlSession>(), configuration);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseMiddleware<ErrorHandlerMiddleware>();
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

const byte MinThreads = 100;

ThreadPool.SetMinThreads(MinThreads, MinThreads);

await app.RunAsync();