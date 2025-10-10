using Fanex.Data.Repository;
using Fanex.Logging;
using GO.Admin.Website.Repositories;
using GO.Admin.Website.Services;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using Microsoft.Extensions.DependencyInjection;

namespace GO.Admin.Website;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterDI(this IServiceCollection services)
    {
        services.AddSingleton<ILogger, Logger>();
        services.AddSingleton<IDynamicRepository, DynamicRepository>();
        services.AddSingleton<IBaseRepository, BaseRepository>();

        services.AddSingleton<IUserControlSession, UserControlSession>();
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<ICryptoService, CryptoService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IRoleService, RoleService>();
        services.AddSingleton<IFunctionService, FunctionService>();

        services.AddSingleton<IBaseService, BaseService>();

        return services;
    }
}