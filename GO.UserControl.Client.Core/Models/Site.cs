namespace GO.UserControl.Client.Core.Models
{
    public class Site
    {
        public short SiteId { get; set; }
        public string SiteName { get; set; }
        public string AccessToken { get; set; }
        public string SiteUrl { get; set; }
        public string SSOUrl { get; set; }
        public bool IsActive { get; set; }
    }
}