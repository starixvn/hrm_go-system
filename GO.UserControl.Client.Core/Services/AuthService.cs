namespace GO.UserControl.Client.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Client;
    using Enumerables;
    using Models.Auth;

    public class AuthService : BaseHttpClient, IAuthService
    {
        public LoginResult Login(LoginParam loginParam)
            => Post<LoginParam, LoginResult>(RelativePaths.LoginPath, loginParam);

        public async Task<LoginResult> LoginAsync(LoginParam loginParam)
            => await PostAsync<LoginParam, LoginResult>(RelativePaths.LoginPath, loginParam);

        public ReCheckInResult ReCheckIn(long onUserId)
        {
            var data = new Dictionary<string, object>
            {
                { "onUserId", onUserId }
            };
            return Post<Dictionary<string, object>, ReCheckInResult>(RelativePaths.ReCheckInPath, data);
        }

        public async Task<ReCheckInResult> ReCheckInAsync(long onUserId)
        {
            var data = new Dictionary<string, object>
            {
                { "onUserId", onUserId }
            };
            return await PostAsync<Dictionary<string, object>, ReCheckInResult>(RelativePaths.ReCheckInPath, data);
        }

        public bool Logout(long onUserId)
        {
            var data = new Dictionary<string, object>
            {
                { "onUserId", onUserId }
            };
            return Post<Dictionary<string, object>, bool>(RelativePaths.LogoutPath, data);
        }

        public async Task<bool> LogoutAsync(long onUserId)
        {
            var data = new Dictionary<string, object>
            {
                { "onUserId", onUserId }
            };
            return await PostAsync<Dictionary<string, object>, bool>(RelativePaths.LogoutPath, data);
        }
        public LoginResult SSOLogin(SSOLoginParam loginParam)
            => Post<SSOLoginParam, LoginResult>(RelativePaths.SSOLoginPath, loginParam);

        public async Task<LoginResult> SSOLoginAsync(SSOLoginParam loginParam)
            => await PostAsync<SSOLoginParam, LoginResult>(RelativePaths.SSOLoginPath, loginParam);

        public string SSOGetTime()
            => Get<string>(RelativePaths.SSOGetTimePath);

        public async Task<string> SSOGetTimeAsync()
            => await GetAsync<string>(RelativePaths.SSOGetTimePath);

        private static class RelativePaths
        {
            public const string LoginPath = "api/auth/login";
            public const string ReCheckInPath = "api/auth/recheckin";
            public const string LogoutPath = "api/auth/logout";
            public const string SSOGetTimePath = "api/sso/gettime";
            public const string SSOLoginPath = "api/sso/login";
        }
    }
}