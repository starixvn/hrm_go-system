namespace GO.UserControl.Client.Core
{
    public interface IUserControlSettings
    {
        short SiteId { get; }

        string AccessToken { get; }

        string ServiceUrl { get; }

        bool EnableReCheckin { get; }
    }
}