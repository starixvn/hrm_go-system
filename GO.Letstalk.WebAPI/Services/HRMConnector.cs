using System.Net.Http;
using System.Threading.Tasks;
using GO.Letstalk.WebAPI.Models;
using Newtonsoft.Json;

namespace GO.Letstalk.WebAPI.Services
{
    public class HRMConnector(IHttpClientFactory httpClientFactory, HRMSettings hrmSettings) : IHRMConnector
    {
        public async Task<CommandResult> Send(string callbackUrl)
        {
            var commandResult = new CommandResult();
            var httpClient = httpClientFactory.CreateClient();
            if (!string.IsNullOrEmpty(hrmSettings.ApiKey))
            {
                httpClient.DefaultRequestHeaders.Add("X-API-Key", hrmSettings.ApiKey);
            }
            using var request = new HttpRequestMessage(HttpMethod.Post, callbackUrl);
            using var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                commandResult = JsonConvert.DeserializeObject<CommandResult>(result);
            }
            else
            {
                commandResult = new CommandResult
                {
                    Code = 1,
                    Message = response.ReasonPhrase
                };
            }
            return commandResult;
        }
    }
}