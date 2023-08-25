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
        private const string _featuresApiUri = "https://etimapi.etim-international.com/api/v2/Feature/Search";
        const string _valueUri = "https://etimapi.etim-international.com/api/v2/Value/Search";

        private readonly HttpClient _httpClient;
        private string _token;
        public ApiRequestService(HttpClient httpClient, string token)
        {
            _httpClient = httpClient;
            _token = token;
        }

        public void SetToken(string token)
        {
            _token = token;
        }

        public ResultOfRequestAllFeatureWithMaximumDetailsAPIv2DTO GetFeatures(RequestAllFeatureWithMaximumDetailsAPIv2DTO request)
        {
            var httpPostRequest = new HttpRequestMessage(HttpMethod.Post, _featuresApiUri)
            {
                Content = JsonContent.Create(request)
            };

            httpPostRequest.Headers.Add("Authorization", _token);

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

        public ResultOfRequestAllValuesWithMaximumDetailsAPIv2DTO GetValues(RequestAllValuesWithMaximumDetailsAPIv2DTO request)
        {
            var httpPostRequest = new HttpRequestMessage(HttpMethod.Post, _valueUri)
            {
                Content = JsonContent.Create(request)
            };

            httpPostRequest.Headers.Add("Authorization", _token);

            HttpResponseMessage response = _httpClient.SendAsync(httpPostRequest).Result;

            var badResult = new ResultOfRequestAllValuesWithMaximumDetailsAPIv2DTO()
            {
                Total = 0,
                Values = new Value[0]
            };

            if (response is not null && response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadFromJsonAsync<ResultOfRequestAllValuesWithMaximumDetailsAPIv2DTO>().Result;
                return result ?? badResult;
            }
            else
            {
                return badResult;
            }
        }
    }
}
