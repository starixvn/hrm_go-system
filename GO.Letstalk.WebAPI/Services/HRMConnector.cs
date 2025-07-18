using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GO.Letstalk.WebAPI.Models;
using Newtonsoft.Json;

namespace GO.Letstalk.WebAPI.Services
{
    public class HRMConnector(IHttpClientFactory httpClientFactory, HRMSettings hrmSettings) : IHRMConnector
    {
        public async Task<CommandResult> Send(string callbackUrl)
        {
            if (callbackUrl.Contains("nexhrm"))
            {
                return await SendNexHRMAsync(callbackUrl);
            }
            return await SendHRMAsync(callbackUrl);
        }

        private async Task<CommandResult> SendHRMAsync(string callbackUrl)
        {
            var commandResult = new CommandResult();

            // Create HttpClient with SSL certificate validation bypass
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            using var httpClient = new HttpClient(handler);

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

        private async Task<CommandResult> SendNexHRMAsync(string callbackUrl)
        {
            var commandResult = new CommandResult();

            var baseUrl = GetBaseUrl(callbackUrl);
            var token = await GetTokenNexHRMAsync(baseUrl);

            // Create HttpClient with SSL certificate validation bypass for NexHRM
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            using var httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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

        private string GetBaseUrl(string callbackUrl)
        {
            try
            {
                var uri = new Uri(callbackUrl);
                return $"{uri.Scheme}://{uri.Authority}";
            }
            catch (UriFormatException)
            {
                var parts = callbackUrl.Split('/');
                return parts[0];
            }
        }
        private async Task<string> GetTokenNexHRMAsync(string baseUrl)
        {
            var url = new Uri($"{baseUrl}/Token/GetToken?username={hrmSettings.NexHRId}&password={hrmSettings.NexHRPassword}");

            // Create HttpClient with SSL certificate validation bypass for NexHRM
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            using var httpClient = new HttpClient(handler);

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            using var response = await httpClient.SendAsync(request);
            var responseData = await response.Content.ReadAsStringAsync();
            return responseData;
        }
    }
}