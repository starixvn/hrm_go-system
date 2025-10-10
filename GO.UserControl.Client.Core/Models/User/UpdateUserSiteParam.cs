namespace GO.UserControl.Client.Core.Models.User
{
    public class UpdateUserSiteParam
    {
        public short SiteId { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; }
    }
}