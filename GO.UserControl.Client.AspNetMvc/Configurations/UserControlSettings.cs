namespace GO.UserControl.Client.AspNetMvc
{
    using System.ComponentModel;
    using System.Configuration;
    using GO.UserControl.Client.Core;

    public class UserControlSettings : IUserControlSettings
    {
        public UserControlSettings()
        {
            SiteId = GetValue<short>("GO.UserControl.Client:SiteId");
            AccessToken = GetValue<string>("GO.UserControl.Client:AccessToken");
            ServiceUrl = GetValue<string>("GO.UserControl.Client:ServiceUrl");
            EnableReCheckin = GetValue<bool>("GO.UserControl.Client:EnableReCheckin");
        }

        public UserControlSettings(short siteId, string accessToken, string serviceUrl, bool enableReCheckin)
        {
            SiteId = siteId;
            AccessToken = accessToken;
            ServiceUrl = serviceUrl;
            EnableReCheckin = enableReCheckin;
        }

        public short SiteId { get; set; }

        public string AccessToken { get; set; }

        public string ServiceUrl { get; set; }

        public bool EnableReCheckin { get; set; }

        private static T GetValue<T>(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));

            return (T)typeConverter.ConvertFromString(value);
        }
    }
}