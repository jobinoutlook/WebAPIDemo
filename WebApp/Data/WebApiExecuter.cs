using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;

namespace WebApp.Data
{
    public class WebApiExecuter : IWebApiExecuter
    {
        private const string apiName = "ShirtsApi";
        private const string authApiName = "AuthorityApi";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WebApiExecuter(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<T?> InvokeGet<T>(string relativeUrl)
        {
            var httpClient = _httpClientFactory.CreateClient(apiName);
            //return await httpClient.GetFromJsonAsync<T>(relativeUrl);
            await AddJwtToHeader(httpClient);
            var request= new HttpRequestMessage(HttpMethod.Get, relativeUrl);
            var response = await httpClient.SendAsync(request);
            await HandlePotentialError(response);

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T?> InvokePost<T>(string relativeUrl, T obj)
        {
            var httpClient = _httpClientFactory.CreateClient(apiName);
            await AddJwtToHeader(httpClient);
            var response = await httpClient.PostAsJsonAsync(relativeUrl, obj);

            await HandlePotentialError(response);

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task InvokePut<T>(string relativeUrl, T obj)
        {
            var httpClient = _httpClientFactory.CreateClient(apiName);
            await AddJwtToHeader(httpClient);
            var response = await httpClient.PutAsJsonAsync(relativeUrl, obj);
            await HandlePotentialError(response);
        }

        public async Task InvokeDelete(string relativeUrl)
        {
            var httpClient = _httpClientFactory.CreateClient(apiName);
            await AddJwtToHeader(httpClient);
            var response = await httpClient.DeleteAsync(relativeUrl);
            await HandlePotentialError(response);
        }

        private async Task HandlePotentialError(HttpResponseMessage httpResponse)
        {
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorJson = await httpResponse.Content.ReadAsStringAsync();
                throw new WebApiException(errorJson);
            }
        }

        private async Task AddJwtToHeader(HttpClient httpClient)
        {
            JwtToken? token = null;
            string? strToken = _httpContextAccessor.HttpContext?.Session.GetString("access_token");
            if(!string.IsNullOrEmpty(strToken?.Trim()))
            {
                token = JsonConvert.DeserializeObject<JwtToken>(strToken);
            }

            if (token == null || token.ExpiresAt <= DateTime.UtcNow)
            {
                var clientId = _configuration["ClientId"];
                var secret = _configuration["Secret"];
                // Authenticate
                var authClient = _httpClientFactory.CreateClient(authApiName);
                var response = await authClient.PostAsJsonAsync("auth", new AppCredential
                {
                    ClientId = clientId ?? string.Empty,
                    Secret = secret ?? string.Empty
                });
                response.EnsureSuccessStatusCode();

                // Get the JWT
                strToken = await response.Content.ReadAsStringAsync();
                token = JsonConvert.DeserializeObject<JwtToken>(strToken);

                _httpContextAccessor.HttpContext?.Session.SetString("access_token", strToken);

            }


            // Add the JWT to the request header

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken);
        }

    }
}
