namespace GO.UserControl.Client.Core.Client
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Jose;
    using Newtonsoft.Json;

    public class BaseHttpClient
    {
        private readonly HttpClient httpClient = new HttpClient();

        protected T Get<T>(string requestUrl)
        {
            ValidateEndPoint(requestUrl);
            using (var request = CreateRequest(requestUrl, HttpMethod.Get))
            {
                using (var response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None).Result)
                {
                    ThrowExceptionWhenNotSuccess(response);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        var result = JsonConvert.DeserializeObject<T>(responseData);

                        return result;
                    }

                    return default;
                }
            }
        }

        protected async Task<T> GetAsync<T>(string requestUrl)
        {
            ValidateEndPoint(requestUrl);
            using (var request = CreateRequest(requestUrl, HttpMethod.Get))
            {
                using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None))
                {
                    ThrowExceptionWhenNotSuccess(response);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(responseData);

                        return result;
                    }

                    return default;
                }
            }
        }

        protected TOutput Post<TInput, TOutput>(string requestUrl, TInput data)
        {
            ValidateEndPoint(requestUrl);
            using (var request = CreateRequest(requestUrl, HttpMethod.Post))
            {
                var encodedData = JsonConvert.SerializeObject(data);
                var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
                request.Content = content;
                using (var response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None).Result)
                {
                    ThrowExceptionWhenNotSuccess(response);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        var result = JsonConvert.DeserializeObject<TOutput>(responseData);

                        return result;
                    }

                    return default;
                }
            }
        }

        protected async Task<TOutput> PostAsync<TInput, TOutput>(string requestUrl, TInput data)
        {
            ValidateEndPoint(requestUrl);
            using (var request = CreateRequest(requestUrl, HttpMethod.Post))
            {
                var encodedData = JsonConvert.SerializeObject(data);
                var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
                request.Content = content;
                using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None))
                {
                    ThrowExceptionWhenNotSuccess(response);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<TOutput>(responseData);

                        return result;
                    }

                    return default;
                }
            }
        }

        private static void ValidateEndPoint(string endPoint)
        {
            if (string.IsNullOrWhiteSpace(endPoint))
            {
                throw new ArgumentNullException(nameof(endPoint));
            }
        }

        private HttpRequestMessage CreateRequest(string endPoint, HttpMethod method)
        {
            var url = new Uri($"{UserControlClient.ServiceUrl}{endPoint}");
            var request = new HttpRequestMessage(method, url);
            request.Headers.Accept.Clear();
            var siteIdentity = GenerateSiteIdentity();
            request.Headers.Add("SiteIdentity", $"Bearer {siteIdentity}");
            if (UserControlClient.Session.JwtToken != null && !string.IsNullOrWhiteSpace(UserControlClient.Session.JwtToken.Token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", UserControlClient.Session.JwtToken.Token);
            }

            return request;
        }

        private static string GenerateSiteIdentity()
        {
            var siteId = UserControlClient.Session.SiteId == 0 ? UserControlClient.SiteId : UserControlClient.Session.SiteId;
            var accessToken = string.IsNullOrWhiteSpace(UserControlClient.Session.AccessToken) ? UserControlClient.AccessToken : UserControlClient.Session.AccessToken;
            var data = new Dictionary<string, object>
            {
                {nameof(UserControlClient.Session.SiteId), siteId },
                {nameof(UserControlClient.Session.AccessToken), accessToken }
            };
            return JWT.Encode(data, Encoding.UTF8.GetBytes(accessToken), JwsAlgorithm.HS256);
        }

        private static void ThrowExceptionWhenNotSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return;
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }

                var message = response.Content.ReadAsStringAsync().Result;
                var exception = new Exception(message);
                throw exception;
            }
        }
    }
}