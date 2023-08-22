using System.Net.Http.Json;
using System.Text.Json;

Console.WriteLine("Hello, I am ETIM API Saver!");

const string _featureUri = "https://etimapi.etim-international.com/api/v2/Feature/Search";
const string _valueUri = "https://etimapi.etim-international.com/api/v2/Value/Search";

var _jsonRequest = new RequestAllFeatureWithMaximumDetailsAPIv2DTO()
{
    From = 0,
    Size = 2,
    Languagecode = "EN",
    Deprecated = false,
    Include = new Include
    {
        Descriptions = true,
        Translations = false
    }
};



HttpClient _httpClient = new HttpClient();

HttpRequestMessage _httpPostRequest = new HttpRequestMessage(HttpMethod.Post, _featureUri)
{
    //Content = JsonContent.Create(_jsonRequest)
};

_httpPostRequest.Content = JsonContent.Create(_jsonRequest);

_httpPostRequest.Headers.Add("Authorization", @"Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IkE4M0EyMTZDRTFCN0YxQUMzRTkzNkM2Qjc1MkE0MkQ4QUE0QjE4NkZSUzI1NiIsInR5cCI6ImF0K2p3dCIsIng1dCI6InFEb2hiT0czOGF3LWsyeHJkU3BDMktwTEdHOCJ9.eyJuYmYiOjE2OTI3MDkzMDMsImV4cCI6MTY5MjcxMjkwMywiaXNzIjoiaHR0cHM6Ly9ldGltYXV0aC5ldGltLWludGVybmF0aW9uYWwuY29tIiwiYXVkIjoiRXRpbUFwaSIsImNsaWVudF9pZCI6InNpZW1lbnNfdWEiLCJjbGllbnRfbGFuZ3VhZ2UiOlsiRU4iLCJubC1CRSIsImZyLUJFIiwiZmktRkkiLCJkZS1ERSIsIml0LUlUIiwibmItTk8iXSwianRpIjoiMEVEODdCNjM3RjgwRENGRUJCNDE1QThDQjlCNkVERDYiLCJpYXQiOjE2OTI3MDkzMDMsInNjb3BlIjpbIkV0aW1BcGkiXX0.GCvTw0-ZXb_GBIAuTna_J2bCLS2WclxBLaKSW7BFgXIIVn9b_UpQ5nYpCHww59ri6BoqySmM52pfKb1iD1ICUEP5IXGTd2Sf61iAX0lrLDN7BpOAroUcyzxaJUwmUg3Ip2RhhkPb3T4KAilWnuxnRAnjYZtOIJaxr3uCdxD9mlEzc05hupA_veup_b44Qrq_3nRwVSfEj5Cp-Om60VBOlQrppxDkbzRskp9dZGrcKP694irlJAZvplA4L1prMZKI1FwOWdwlO0pmv-qjZA_na4k3aA4v8wrFLye6jfgRLPblSu7VwI_iV2pL_UVmm0y0ii7njgIcUJjPppYfV2FkGg");
//_httpPostRequest.Headers.Add("Content-Type", "application/json");
//_httpPostRequest.Headers.Add("Accept", "*/*");                        // not necessarily
//_httpPostRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br"); // not necessarily
//_httpPostRequest.Headers.Add("Connection", "keep-alive");             // not necessarily
//_httpPostRequest.Headers.Add("Host", "etimapi.etim-international.com");
//_httpPostRequest.Headers.Add("Content-Length", "147");


var _response = _httpClient.SendAsync(_httpPostRequest).Result;

ResultOfRequestAllFeatureWithMaximumDetailsAPIv2DTO? result = null;

if (_response.IsSuccessStatusCode)
{
    Console.WriteLine("Ok");
    try
    {
        result = _response.Content.ReadFromJsonAsync<ResultOfRequestAllFeatureWithMaximumDetailsAPIv2DTO>().Result;
    }
    catch(NotSupportedException)
    {
        Console.WriteLine("Content type is not supported");
    }
    catch (JsonException)
    {
        Console.WriteLine("Invalid json");
    }
}

if (result is not null)
{
    Console.WriteLine(result.total);
}


Console.WriteLine("end");
Console.ReadKey();




public class RequestAllFeatureWithMaximumDetailsAPIv2DTO
{
    public int From { get; set; }
    // Can't be more than 1000
    public int Size { get; set; }
    // "EN"
    public string Languagecode { get; set; }
    // false
    public bool Deprecated { get; set; }
    public Include Include { get; set; }
}
public class RequestAllValuesWithMaximumDetailsAPIv2DTO
{
    public int From { get; set; }
    // Can't be more than 1000
    public int Size { get; set; }
    // "EN"
    public string Languagecode { get; set; }
    // false
    public bool Deprecated { get; set; }
    public Include Include { get; set; }
}

public class Include
{
    // true
    public bool Descriptions { get; set; }
    // false
    public bool Translations { get; set; }
}

public class ResultOfRequestAllFeatureWithMaximumDetailsAPIv2DTO
{
    public int total { get; set; }
    public Feature[] features { get; set; }
}

public class Feature
{
    public string code { get; set; }
    public string type { get; set; }
    public bool deprecated { get; set; }
    public string description { get; set; }
}

public class ResultOfRequestAllValuesWithMaximumDetailsAPIv2DTO
{
    public int total { get; set; }
    public Value[] values { get; set; }
}

public class Value
{
    public string code { get; set; }
    public bool deprecated { get; set; }
    public string description { get; set; }
}



