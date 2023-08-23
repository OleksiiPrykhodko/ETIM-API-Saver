using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.Json;
using Common.DTO;
using Common.DTO.Request;
using Common.DTO.Result;


Console.WriteLine("Hello, I am ETIM API Saver!");
Console.WriteLine();

const string _authorizationUri = "https://etimauth.etim-international.com/connect/token";
const string _featureUri = "https://etimapi.etim-international.com/api/v2/Feature/Search";
const string _valueUri = "https://etimapi.etim-international.com/api/v2/Value/Search";

const string _directoryForFilesPath = @"./Generated files";

string _authorization = "Basic c2llbWVuc191YTpYMGV5Y0hRWXFjUW9neU5GZ1ZRQ3lE";

HttpClient _httpClient = new HttpClient();

var _listOfAllFeatures = new List<Feature>();

int _totalNumberOfFeatures = 0;
int _numberOfAlreadyLoadedFeatures = 0;

// STEP ONE - get access token ======================================================
Console.WriteLine("Authorization step is started.");

HttpRequestMessage _httpPostAuthorizationRequest = new HttpRequestMessage(HttpMethod.Post, _authorizationUri)
{
    Content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
    {
        new KeyValuePair<string, string>("grant_type", "client_credentials"),
        new KeyValuePair<string, string>("scope", "EtimApi")
    })
};

_httpPostAuthorizationRequest.Headers.Add("Authorization", _authorization);

HttpResponseMessage? _authorizationResponse = null;

try
{
   _authorizationResponse = _httpClient.SendAsync(_httpPostAuthorizationRequest).Result;
}
catch (Exception e)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Error! {e.Message}");
    Console.WriteLine("Check internet connection and try again!");
}
finally
{
    Console.ResetColor();
}

ResultOfRequestAccessToken? _authorizationResult = null;

if (_authorizationResponse is not null && _authorizationResponse.IsSuccessStatusCode)
{
    Console.WriteLine("Authorization successful.");
    try
    {
        _authorizationResult = _authorizationResponse.Content.ReadFromJsonAsync<ResultOfRequestAccessToken>().Result;
    }
    catch (NotSupportedException)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Error! Content type is not supported.");
    }
    catch (JsonException)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Error! Invalid json.");
    }
    catch (Exception e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error! {e.Message}");
    }
    finally
    {
        Console.ResetColor();
    }
}
else
{
    Console.WriteLine("Authorization is failed.");
}

Console.WriteLine("Authorization step is ended.");
Console.WriteLine();


// STEP TWO - get data ======================================================
Console.WriteLine("Data loading step is started.");



if (_authorizationResult is not null)
{
    do
    {

        var _jsonRequest = new RequestAllFeatureWithMaximumDetailsAPIv2DTO()
        {
            From = _numberOfAlreadyLoadedFeatures,
            Size = 1000,
            Languagecode = "EN",
            Deprecated = false,
            Include = new Include
            {
                Descriptions = true,
                Translations = false
            }
        };

        var _httpPostRequest = new HttpRequestMessage(HttpMethod.Post, _featureUri)
        {
            Content = JsonContent.Create(_jsonRequest)
        };

        string _token = $"Bearer {_authorizationResult?.Access_token ?? "no token"}";
        _httpPostRequest.Headers.Add("Authorization", _token);

        HttpResponseMessage? _response = null;

        try
        {
            _response = _httpClient.SendAsync(_httpPostRequest).Result;
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error! {e.Message}");
            Console.WriteLine("Check internet connection and try again!");
        }
        finally
        {
            Console.ResetColor();
        }

        ResultOfRequestAllFeatureWithMaximumDetailsAPIv2DTO? _result = null;

        if (_response is not null && _response.IsSuccessStatusCode)
        {
            Console.WriteLine("Data was loaded successful.");
            try
            {
                _result = _response.Content.ReadFromJsonAsync<ResultOfRequestAllFeatureWithMaximumDetailsAPIv2DTO>().Result;
            }
            catch (NotSupportedException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error! Content type is not supported.");
            }
            catch (JsonException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error! Invalid json.");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error! {e.Message}");
            }
            finally
            {
                Console.ResetColor();
            }
        }
        else
        {
            Console.WriteLine("Data loading is failed.");
            break;
        }

        if (_result is not null)
        {
            _totalNumberOfFeatures = _result.Total;
            _listOfAllFeatures.AddRange(_result.Features);
            _numberOfAlreadyLoadedFeatures = _listOfAllFeatures.Count;
        }
        else
        {
            break;
        }

    }
    while (_numberOfAlreadyLoadedFeatures < _totalNumberOfFeatures);

    Console.WriteLine($"{_totalNumberOfFeatures}/{_numberOfAlreadyLoadedFeatures} - Features (total number / loaded).");
}
else
{
    Console.WriteLine("Data loading step skipped. Downloading is not possible without authorization!");
}

Console.WriteLine("Data loading step is ended.");
Console.WriteLine();


// STEP THREE - save data to file ======================================================
Console.WriteLine("Data in file saving step is started.");


// If directory does not exist, create it
if (!Directory.Exists(_directoryForFilesPath))
{
    Directory.CreateDirectory(_directoryForFilesPath);
}




Console.WriteLine("The End");
Console.ReadKey();



















