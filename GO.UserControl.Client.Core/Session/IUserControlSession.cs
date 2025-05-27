namespace GO.UserControl.Client.Core.Session
{
    using System.Collections.Generic;
    using Enumerables;
    using Models;
    using Models.Auth;

    public interface IUserControlSession
    {
        short SiteId { get; set; }
        string AccessToken { get; set; }

        int UserId { get; set; }
        long OnUserId { get; set; }
        JwtToken JwtToken { get; set; }
        IList<FunctionPermission> Functions { get; set; }
        IList<MenuItem> MenuItems { get; set; }

        T Get<T>(string key);

        void Set<T>(string key, T value);

        bool HasPermissionOnFunction(string functionKey, Policy policy);

        void ClearAll();
    }
}