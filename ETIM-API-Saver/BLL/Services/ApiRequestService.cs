using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Common.DTO;
using Common.DTO.Request;
using Common.DTO.Result;

namespace BLL.Services
{
    public class ApiRequestService
    {
        private const string _featureUri = "https://etimapi.etim-international.com/api/v2/Feature/Search";

        private readonly HttpClient _httpClient;

        public ApiRequestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public ResultOfRequestAllFeatureWithMaximumDetailsAPIv2DTO GetFeatures(RequestAllFeatureWithMaximumDetailsAPIv2DTO request, string token)
        {

            var httpPostRequest = new HttpRequestMessage(HttpMethod.Post, _featureUri)
            {
                Content = JsonContent.Create(request)
            };

            httpPostRequest.Headers.Add("Authorization", token);

            HttpResponseMessage response = _httpClient.SendAsync(httpPostRequest).Result;

            var badResult = new ResultOfRequestAllFeatureWithMaximumDetailsAPIv2DTO()
            {
                Total = 0,
                Features = new Feature[0]
            };

            if (response is not null && response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadFromJsonAsync<ResultOfRequestAllFeatureWithMaximumDetailsAPIv2DTO>().Result;
                return result ?? badResult;
            }
            else
            {
                return badResult;
            }
        }
    }
}
