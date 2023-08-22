using System.Net.Http.Json;
using System.Text.Json;

Console.WriteLine("Hello, I am ETIM API Saver!");
const string _authorizationUri = "https://etimauth.etim-international.com/connect/token";
const string _featureUri = "https://etimapi.etim-international.com/api/v2/Feature/Search";
const string _valueUri = "https://etimapi.etim-international.com/api/v2/Value/Search";
HttpClient _httpClient = new HttpClient();

// STEP ONE - get access token

string _authorization = "Basic c2llbWVuc191YTpYMGV5Y0hRWXFjUW9neU5GZ1ZRQ3lE";

HttpRequestMessage _httpPostAuthorizationRequest = new HttpRequestMessage(HttpMethod.Post, _authorizationUri)
{
    Content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
    {
        new KeyValuePair<string, string>("grant_type", "client_credentials"),
        new KeyValuePair<string, string>("scope", "EtimApi")
    })
};

_httpPostAuthorizationRequest.Headers.Add("Authorization", _authorization);

var _authorizationResponse = _httpClient.SendAsync(_httpPostAuthorizationRequest).Result;

ResultOfRequestAccessToken? _authorizationResult = null;

if (_authorizationResponse.IsSuccessStatusCode)
{
    Console.WriteLine("Ok");
    try
    {
        _authorizationResult = _authorizationResponse.Content.ReadFromJsonAsync<ResultOfRequestAccessToken>().Result;
    }
    catch (NotSupportedException)
    {
        Console.WriteLine("Content type is not supported");
    }
    catch (JsonException)
    {
        Console.WriteLine("Invalid json");
    }
}



Console.WriteLine("auth end");


// STEP TWO - get data

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

HttpRequestMessage _httpPostRequest = new HttpRequestMessage(HttpMethod.Post, _featureUri)
{
    //Content = JsonContent.Create(_jsonRequest)
};

_httpPostRequest.Content = JsonContent.Create(_jsonRequest);

string _token =$"Bearer {_authorizationResult?.Access_token ?? "no token"}";
_httpPostRequest.Headers.Add("Authorization", _token);
//_httpPostRequest.Headers.Add("Content-Type", "application/json");
//_httpPostRequest.Headers.Add("Accept", "*/*");                        // not necessarily
//_httpPostRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br"); // not necessarily
//_httpPostRequest.Headers.Add("Connection", "keep-alive");             // not necessarily
//_httpPostRequest.Headers.Add("Host", "etimapi.etim-international.com");
//_httpPostRequest.Headers.Add("Content-Length", "147");

var _response = _httpClient.SendAsync(_httpPostRequest).Result;

ResultOfRequestAllFeatureWithMaximumDetailsAPIv2DTO? _result = null;

if (_response.IsSuccessStatusCode)
{
    Console.WriteLine("Ok");
    try
    {
        _result = _response.Content.ReadFromJsonAsync<ResultOfRequestAllFeatureWithMaximumDetailsAPIv2DTO>().Result;
    }
    catch (NotSupportedException)
    {
        Console.WriteLine("Content type is not supported");
    }
    catch (JsonException)
    {
        Console.WriteLine("Invalid json");
    }
}

if (_result is not null)
{
    Console.WriteLine(_result.Total);
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
    public int Total { get; set; }
    public Feature[] Features { get; set; }
}

public class Feature
{
    public string Code { get; set; }
    public string Type { get; set; }
    public bool Deprecated { get; set; }
    public string Description { get; set; }
}

public class ResultOfRequestAllValuesWithMaximumDetailsAPIv2DTO
{
    public int Total { get; set; }
    public Value[] Values { get; set; }
}

public class Value
{
    public string Code { get; set; }
    public bool Deprecated { get; set; }
    public string Description { get; set; }
}

// access_token

public class ResultOfRequestAccessToken
{
    public string Access_token { get; set; }
    public int Expires_in { get; set; }
    public string Token_type { get; set; }
    public string Scope { get; set; }
}


