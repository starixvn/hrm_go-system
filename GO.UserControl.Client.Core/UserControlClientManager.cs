namespace GO.UserControl.Client.Core
{
    using System;
    using GO.UserControl.Client.Core;
    using GO.UserControl.Client.Core.Session;

    public class UserControlClientManager
    {
        private IUserControlSession userControlSession;
        private IUserControlSettings userControlSettings;
        private static UserControlClientManager CurrentInstance = new UserControlClientManager();

        public static UserControlClientManager UseSettings(IUserControlSettings settings)
        {
            CurrentInstance.userControlSettings = settings ?? throw new ArgumentNullException(nameof(settings));

            return CurrentInstance;
        }

        public UserControlClientManager UseSession(IUserControlSession userControlSession)
        {
            CurrentInstance.userControlSession = userControlSession ?? throw new ArgumentNullException(nameof(userControlSession));

            return CurrentInstance;
        }

        public static IUserControlSettings UserControlSettings
            => CurrentInstance.userControlSettings ?? throw new InvalidOperationException(nameof(CurrentInstance.userControlSettings));

        public static IUserControlSession UserControlSession
            => CurrentInstance.userControlSession ?? throw new InvalidOperationException(nameof(CurrentInstance.userControlSession));
    }
}