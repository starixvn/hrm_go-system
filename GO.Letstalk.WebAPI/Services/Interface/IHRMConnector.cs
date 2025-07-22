using System.Threading.Tasks;
using GO.Letstalk.WebAPI.Models;

namespace GO.Letstalk.WebAPI.Services
{
    public interface IHRMConnector
    {
        Task<CommandResult> Send(string callbackUrl);
    }
}