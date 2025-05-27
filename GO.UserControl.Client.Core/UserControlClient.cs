namespace GO.UserControl.Client.Core
{
    using System;
    using Session;

#if NETFRAMEWORK
    using System.Configuration;
    using System.ComponentModel;
#endif
#if NET8_0_OR_GREATER
    using System.ComponentModel;
    using Microsoft.Extensions.Configuration;
#endif

    public class UserControlClient
    {
        public static short SiteId { get; private set; }
        public static string AccessToken { get; private set; }
        public static string ServiceUrl { get; private set; }
        public static bool EnableReCheckin { get; private set; }
        public static IUserControlSession Session { get; private set; }

#if NETFRAMEWORK
        public static void UseSession(IUserControlSession session)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));

            SiteId = GetValue<short>("GO.UserControl.Client:SiteId");
            AccessToken = GetValue<string>("GO.UserControl.Client:AccessToken");
            ServiceUrl = GetValue<string>("GO.UserControl.Client:ServiceUrl");
            EnableReCheckin = GetValue<bool>("GO.UserControl.Client:EnableReCheckin");
        }

        private static T GetValue<T>(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            var typeConverter = TypeDescriptor.GetConverter(typeof(T));

            return (T)typeConverter.ConvertFromString(value);
        }
#elif NET8_0_OR_GREATER
        public static void UseSession(IUserControlSession session, IConfiguration configuration)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));

            SiteId = GetValue<short>(configuration, "GO.UserControl.Client:SiteId");
            AccessToken = GetValue<string>(configuration, "GO.UserControl.Client:AccessToken");
            ServiceUrl = GetValue<string>(configuration, "GO.UserControl.Client:ServiceUrl");
            EnableReCheckin = GetValue<bool>(configuration, "GO.UserControl.Client:EnableReCheckin");
        }

        private static T GetValue<T>(IConfiguration configuration, string key)
        {
            var value = configuration.GetSection(key).Value;

            var typeConverter = TypeDescriptor.GetConverter(typeof(T));

            return (T)typeConverter.ConvertFromString(value);
        }
#endif
    }
}