namespace GO.UserControl.Client.AspNetCore
{
    using System;
    using System.ComponentModel;
    using GO.UserControl.Client.Core;
    using Microsoft.Extensions.Configuration;

    public class UserControlSettings : IUserControlSettings
    {
        private readonly IConfiguration configuration;

        public UserControlSettings(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            SiteId = GetValue<short>("GO.UserControl.Client:SiteId");
            AccessToken = GetValue<string>("GO.UserControl.Client:AccessToken");
            ServiceUrl = GetValue<string>("GO.UserControl.Client:ServiceUrl");
            EnableReCheckin = GetValue<bool>("GO.UserControl.Client:EnableReCheckin");
        }

        public short SiteId { get; }

        public string AccessToken { get; }

        public string ServiceUrl { get; }

        public bool EnableReCheckin { get; set; }

        private T GetValue<T>(string key)
        {
            var value = configuration.GetSection(key).Value;

            var typeConverter = TypeDescriptor.GetConverter(typeof(T));

            return (T)typeConverter.ConvertFromString(value);
        }
    }
}