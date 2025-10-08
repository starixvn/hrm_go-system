using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using GO.Letstalk.WebAPI.Models;
using Newtonsoft.Json;

namespace GO.Letstalk.WebAPI.Services;

public class HRMConnector(HRMSettings hrmSettings) : IHRMConnector
{
    public async Task<CommandResult> Send(string callbackUrl)
    {
        if (callbackUrl.Contains("nexhrm"))
        {
            return await SendNexHrmAsync(callbackUrl);
        }

        return await SendHrmAsync(callbackUrl);
    }

    private async Task<CommandResult> SendHrmAsync(string callbackUrl)
    {
        CommandResult commandResult;

        // Create HttpClient with SSL certificate validation bypass
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = ServerCertificateCustomValidationCallback
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

    private async Task<CommandResult> SendNexHrmAsync(string callbackUrl)
    {
        CommandResult commandResult;

        var baseUrl = GetBaseUrl(callbackUrl);
        var token = await GetTokenNexHrmAsync(baseUrl);

        // Create HttpClient with SSL certificate validation bypass for NexHRM
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = ServerCertificateCustomValidationCallback
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

    private static string GetBaseUrl(string callbackUrl)
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
    private async Task<string> GetTokenNexHrmAsync(string baseUrl)
    {
        var url = new Uri($"{baseUrl}/Token/GetToken?username={hrmSettings.NexHRId}&password={hrmSettings.NexHRPassword}");

        // Create HttpClient with SSL certificate validation bypass for NexHRM
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = ServerCertificateCustomValidationCallback
        };
        using var httpClient = new HttpClient(handler);

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        using var response = await httpClient.SendAsync(request);
        var responseData = await response.Content.ReadAsStringAsync();
        return responseData;
    }

    private static bool ServerCertificateCustomValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
}