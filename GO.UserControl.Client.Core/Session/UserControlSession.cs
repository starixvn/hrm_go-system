namespace GO.UserControl.Client.Core.Session
{
    using System.Collections.Generic;
    using System.Linq;
    using Constants;
    using Enumerables;
    using Models;
    using Models.Auth;
    using Newtonsoft.Json;

#if NETFRAMEWORK
    using System.Web;
    using System.Web.SessionState;

    public class UserControlSession : IUserControlSession, IRequiresSessionState
    {
        private static HttpSessionState Session => HttpContext.Current?.Session;

        public string AccessToken
        {
            get => Session[SessionKey.SessionAccessToken]?.ToString();
            set => Session.Add(SessionKey.SessionAccessToken, value);
        }

        public int UserId
        {
            get => int.Parse(Session[SessionKey.SessionUserId]?.ToString() ?? "0");
            set => Session.Add(SessionKey.SessionUserId, value);
        }

        public long OnUserId
        {
            get => long.Parse(Session[SessionKey.SessionOnUserId]?.ToString() ?? "0");
            set => Session.Add(SessionKey.SessionOnUserId, value);
        }

        public short SiteId
        {
            get => short.Parse(Session[SessionKey.SessionSiteId]?.ToString() ?? "0");
            set => Session.Add(SessionKey.SessionSiteId, value);
        }

        public JwtToken JwtToken
        {
            get
            {
                if (Session[SessionKey.SessionJwtToken] != null)
                {
                    return JsonConvert.DeserializeObject<JwtToken>(Session[SessionKey.SessionJwtToken]?.ToString());
                }

                return new JwtToken();
            }
            set => Session.Add(SessionKey.SessionJwtToken, JsonConvert.SerializeObject(value));
        }

        public IList<FunctionPermission> Functions
        {
            get
            {
                if (Session[SessionKey.SessionFunctions] != null)
                {
                    return JsonConvert.DeserializeObject<IList<FunctionPermission>>(Session[SessionKey.SessionFunctions]?.ToString());
                }

                return new List<FunctionPermission>();
            }
            set => Session.Add(SessionKey.SessionFunctions, JsonConvert.SerializeObject(value));
        }

        public IList<MenuItem> MenuItems
        {
            get
            {
                if (Session[SessionKey.SessionMenuItems] != null)
                {
                    return JsonConvert.DeserializeObject<IList<MenuItem>>(Session[SessionKey.SessionMenuItems]?.ToString());
                }

                return new List<MenuItem>();
            }
            set => Session.Add(SessionKey.SessionMenuItems, JsonConvert.SerializeObject(value));
        }

        public T Get<T>(string key)
        {
            if (Session[key] != null)
            {
                return JsonConvert.DeserializeObject<T>(Session[key]?.ToString());
            }

            return default(T);
        }

        public void Set<T>(string key, T value)
        {
            Session.Add(key, JsonConvert.SerializeObject(value));
        }

        public bool HasPermissionOnFunction(string functionKey, Policy policy)
        {
            var result = false;

            if (Functions != null && Functions.Any(x => x.FunctionKey == functionKey))
            {
                switch (policy)
                {
                    case Policy.Read:
                        result = Functions.Any(x => x.FunctionKey == functionKey && (x.IsRead || x.IsHost));
                        break;

                    case Policy.Host:
                        result = Functions.Any(x => x.FunctionKey == functionKey && x.IsHost);
                        break;

                    default:
                        result = false;
                        break;
                }
            }

            return result;
        }

        public void ClearAll()
        {
            Session.Abandon();
            Session.RemoveAll();
        }
    }

#elif NET8_0_OR_GREATER
    using Microsoft.AspNetCore.Http;

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
#endif
}