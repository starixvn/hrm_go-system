namespace GO.UserControl.Client.AspNetCore.Session
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Constants;
    using Core.Enumerables;
    using Core.Models;
    using Core.Models.Auth;
    using GO.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public class UserControlSession(IHttpContextAccessor accessor) : IUserControlSession
    {
        public short SiteId
        {
            get => accessor.HttpContext.Session?.Get<short>(SessionKey.SessionSiteId) ?? 0;
            set => accessor.HttpContext.Session.Set<short>(SessionKey.SessionSiteId, value);
        }

        public string AccessToken
        {
            get => accessor.HttpContext.Session?.GetString(SessionKey.SessionAccessToken);
            set => accessor.HttpContext.Session.SetString(SessionKey.SessionAccessToken, value);
        }

        public int UserId
        {
            get => accessor.HttpContext.Session?.GetInt32(SessionKey.SessionUserId) ?? 0;
            set => accessor.HttpContext.Session.SetInt32(SessionKey.SessionUserId, value);
        }

        public long OnUserId
        {
            get => accessor.HttpContext.Session?.Get<long>(SessionKey.SessionOnUserId) ?? 0;
            set => accessor.HttpContext.Session.Set<long>(SessionKey.SessionOnUserId, value);
        }

        public JwtToken JwtToken
        {
            get => accessor.HttpContext.Session?.Get<JwtToken>(SessionKey.SessionJwtToken);
            set => accessor.HttpContext.Session.Set(SessionKey.SessionJwtToken, value);
        }

        public IList<FunctionPermission> Functions
        {
            get => accessor.HttpContext.Session.Get<List<FunctionPermission>>(SessionKey.SessionFunctions);
            set => accessor.HttpContext.Session.Set(SessionKey.SessionFunctions, value);
        }

        public IList<MenuItem> MenuItems
        {
            get => accessor.HttpContext.Session.Get<List<MenuItem>>(SessionKey.SessionMenuItems);
            set => accessor.HttpContext.Session.Set(SessionKey.SessionMenuItems, value);
        }

        public T Get<T>(string key) => accessor.HttpContext.Session.Get<T>(key);

        public void Set<T>(string key, T value) => accessor.HttpContext.Session.Set(key, value);

        public bool HasPermissionOnFunction(string functionKey, Policy policy)
        {
            var result = false;
            if (Functions != null && Functions.Any(x => x.FunctionKey == functionKey))
            {
                result = policy switch
                {
                    Policy.Read => Functions.Any(x => x.FunctionKey == functionKey && (x.IsRead || x.IsHost)),
                    Policy.Host => Functions.Any(x => x.FunctionKey == functionKey && x.IsHost),
                    _ => false
                };
            }

            return result;
        }

        public void ClearAll()
        {
            accessor.HttpContext.Session.Clear();
        }
    }

    internal static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}