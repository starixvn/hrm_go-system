namespace GO.UserControl.Client.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.UserControl.Client.Core.Client;
    using GO.UserControl.Client.Core.Models;
    using GO.UserControl.Client.Core.Models.User;

    public class UserService : IUserService
    {
        private readonly UserClient userClient;

        public UserService()
        {
            userClient = new UserClient();
        }

        public IList<MenuItem> GetMenuItem()
            => userClient.GetMenuItem();

        public async Task<IList<MenuItem>> GetMenuItemAsync()
             => await userClient.GetMenuItemAsync();

        public IList<FunctionPermission> GetFunctionPermission(short siteId, int userId)
            => userClient.GetFunctionPermission(siteId, userId);

        public async Task<IList<FunctionPermission>> GetFunctionPermissionAsync(short siteId, int userId)
             => await userClient.GetFunctionPermissionAsync(siteId, userId);

        public IList<User> GetUserBySiteId(string keyword = null, byte? statusId = null)
            => userClient.GetUserBySiteId(keyword, statusId);

        public async Task<IList<User>> GetUserBySiteIdAsync(string keyword = null, byte? statusId = null)
            => await userClient.GetUserBySiteIdAsync(keyword, statusId);

        public User GetById(int id)
            => userClient.GetById(id);

        public async Task<User> GetByIdAsync(int id)
            => await userClient.GetByIdAsync(id);

        public CommandResult Create(CreateUserParam param)
            => userClient.Create(param);

        public async Task<CommandResult> CreateAsync(CreateUserParam param)
            => await userClient.CreateAsync(param);

        public CommandResult UpdateInfo(UpdateUserInfoParam param)
            => userClient.UpdateInfo(param);

        public async Task<CommandResult> UpdateInfoAsync(UpdateUserInfoParam param)
            => await userClient.UpdateInfoAsync(param);

        public CommandResult AssignRole(AssignRoleParam param)
            => userClient.AssignRole(param);

        public async Task<CommandResult> AssignRoleAsync(AssignRoleParam param)
            => await userClient.AssignRoleAsync(param);

        public CommandResult UpdatePassword(UpdatePasswordParam param)
            => userClient.UpdatePassword(param);

        public async Task<CommandResult> UpdatePasswordAsync(UpdatePasswordParam param)
            => await userClient.UpdatePasswordAsync(param);

        public CommandResult ResetPassword(UpdatePasswordParam param)
            => userClient.ResetPassword(param);

        public async Task<CommandResult> ResetPasswordAsync(UpdatePasswordParam param)
            => await userClient.ResetPasswordAsync(param);

        public CommandResult Delete(int userId)
            => userClient.Delete(userId);

        public async Task<CommandResult> DeleteAsync(int userId)
            => await userClient.DeleteAsync(userId);
    }
}