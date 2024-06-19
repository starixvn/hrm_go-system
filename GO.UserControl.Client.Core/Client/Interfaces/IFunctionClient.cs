namespace GO.UserControl.Client.Core.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.UserControl.Client.Core.Models;

    public interface IFunctionClient
    {
        IList<Function> GetBySiteId(short siteId, bool? isActive = true);

        Task<IList<Function>> GetBySiteIdAsync(short siteId, bool? isActive = true);

        IList<FunctionPermission> GetPermissionBySiteAndRoleId(short siteId, short roleId);

        Task<IList<FunctionPermission>> GetPermissionBySiteAndRoleIdAsync(short siteId, short roleId);
    }
}