namespace GO.UserControl.Client.Core
{
    public interface IUserControlSettings
    {
        short SiteId { get; set; }

        string AccessToken { get; set; }

        string ServiceUrl { get; set; }

        bool EnableReCheckin { get; set; }
    }
}