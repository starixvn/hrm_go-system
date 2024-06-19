namespace GO.UserControl.Client.Core.Client
{
    using System.Threading.Tasks;
    using GO.UserControl.Client.Core.Models.Auth;

    public interface ISSOClient
    {
        string GetTime();

        Task<string> GetTimeAsync();

        LoginResult Login(SSOLoginParam loginParam);

        Task<LoginResult> LoginAsync(SSOLoginParam loginParam);
    }
}