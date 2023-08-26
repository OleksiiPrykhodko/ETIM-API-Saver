using Common.DTO.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ApiAuthorizationService
    {
        private const string _apiAuthorizationUri = "https://etimauth.etim-international.com/connect/token";

        private readonly HttpClient _httpClient;
        public ApiAuthorizationService(HttpClient httpClient)
        { 
            _httpClient = httpClient;
        }

        public string GetAuthorizationToken(string clientId, string clientSecret) 
        {
            HttpRequestMessage httpPostAuthorizationRequest = new HttpRequestMessage(HttpMethod.Post, _apiAuthorizationUri)
            {
                Content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("scope", "EtimApi")
                })
            };

            var authorization = CreateAuthorizationString(clientId, clientSecret);

            httpPostAuthorizationRequest.Headers.Add("Authorization", authorization);

            var authorizationResponse = _httpClient.SendAsync(httpPostAuthorizationRequest).Result;

            if (authorizationResponse is not null && authorizationResponse.IsSuccessStatusCode)
            {
                var authorizationResult = authorizationResponse.Content.ReadFromJsonAsync<ResultOfRequestAccessTokenDTO>().Result;
                var accessToken = $"Bearer {authorizationResult?.Access_token ?? "no token"}";

                return accessToken;
            }
            else
            {
                return string.Empty;
            }
        }

        private string CreateAuthorizationString(string clientId, string clientSecret)
        {
            var textBytes = Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
            var textBytesAsString = Convert.ToBase64String(textBytes);
            return $"Basic {textBytesAsString}";
        }
    }

}
