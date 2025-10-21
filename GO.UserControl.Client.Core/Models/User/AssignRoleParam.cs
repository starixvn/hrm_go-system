namespace GO.UserControl.Client.Core.Models.User
{
    using System;

    [Serializable]
    public class AssignRoleParam
    {
        public short SiteId { get; set; }
        public int UserId { get; set; }
        public string RoleIds { get; set; }
    }
}