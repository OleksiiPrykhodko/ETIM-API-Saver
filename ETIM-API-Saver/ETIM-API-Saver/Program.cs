using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.Json;
using System.Xml.Serialization;
using BLL.Services;
using Common.DTO;
using Common.DTO.Request;
using Common.DTO.Result;
using static System.Net.Mime.MediaTypeNames;


Console.WriteLine("Hello, I am ETIM API Saver!");
Console.WriteLine();

const string _clientId = "siemens_ua";
const string _clientSecret = "X0eycHQYqcQogyNFgVQCyD";

const string _authorizationUri = "https://etimauth.etim-international.com/connect/token";
const string _featureUri = "https://etimapi.etim-international.com/api/v2/Feature/Search";
const string _valueUri = "https://etimapi.etim-international.com/api/v2/Value/Search";

const string _pathToFilesDirectory = @"./Generated files";

string _token = "";

HttpClient _httpClient = new HttpClient();

ApiRequestService _apiService = new ApiRequestService(_httpClient);

var _listOfAllFeatures = new List<Feature>();
var _listofAllValues = new List<Value>();

int _totalNumberOfFeatures = 0;
int _totalNumberOfValues = 0;

int _numberOfAlreadyLoadedFeatures = 0;
int _numberOfAlreadyLoadedValues = 0;

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


var _authorization = CreateAuthorizationString(_clientId, _clientSecret);

_httpPostAuthorizationRequest.Headers.Add("Authorization", _authorization);

HttpResponseMessage? _authorizationResponse = null;

try
{
   _authorizationResponse = _httpClient.SendAsync(_httpPostAuthorizationRequest).Result;
}
catch (Exception e)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\tError! {e.Message}");
    Console.WriteLine("\tCheck internet connection and try again!");
}
finally
{
    Console.ResetColor();
}

ResultOfRequestAccessTokenDTO? _authorizationResult = null;

if (_authorizationResponse is not null && _authorizationResponse.IsSuccessStatusCode)
{
    Console.WriteLine("\tAuthorization successful.");
    try
    {
        _authorizationResult = _authorizationResponse.Content.ReadFromJsonAsync<ResultOfRequestAccessTokenDTO>().Result;
        _token = $"Bearer {_authorizationResult?.Access_token ?? "no token"}";
    }
    catch (NotSupportedException)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\tError! Content type is not supported.");
    }
    catch (JsonException)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\tError! Invalid json.");
    }
    catch (Exception e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\tError! {e.Message}");
    }
    finally
    {
        Console.ResetColor();
    }
}
else
{
    Console.WriteLine("\tAuthorization is failed.");
}

Console.WriteLine("Authorization step is ended.");
Console.WriteLine();


// STEP TWO - get Features ======================================================
Console.WriteLine("Loading of Features step is started.");

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

        ResultOfRequestAllFeatureWithMaximumDetailsAPIv2DTO _loadedFeatures =
            new ResultOfRequestAllFeatureWithMaximumDetailsAPIv2DTO()
            {
                Total = 0,
                Features = new Feature[0]
            };

        try
        {
            _loadedFeatures = _apiService.GetFeatures(_jsonRequest, _token);
            Console.ForegroundColor = ConsoleColor.Red;
        }
        catch (AggregateException ex)
        {
            Console.WriteLine($"\tError! {ex.Message}");
            Console.WriteLine("\tCheck internet connection and try again!");
        }
        catch (NotSupportedException ex)
        {
            Console.WriteLine($"\tError! {ex.Message}");
            Console.WriteLine("\tError! Content type is not supported.");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"\tError! {ex.Message}");
            Console.WriteLine("\tError! Invalid json.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\tError! {ex.Message}");
        }
        finally
        {
            Console.ResetColor();
        }

        if (_loadedFeatures.Features.Length > 0)
        {
            _totalNumberOfFeatures = _loadedFeatures.Total;
            _listOfAllFeatures.AddRange(_loadedFeatures.Features);
            _numberOfAlreadyLoadedFeatures = _listOfAllFeatures.Count;

            Console.WriteLine($"\t{_numberOfAlreadyLoadedFeatures} Features was loaded successful.");
        }
        else 
        {
            Console.WriteLine("\tFeatures loading is failed.");
            break; 
        }

    }
    while (_numberOfAlreadyLoadedFeatures < _totalNumberOfFeatures);

    Console.WriteLine($"\t{_totalNumberOfFeatures}/{_numberOfAlreadyLoadedFeatures} - Features (total number / loaded).");
}
else
{
    Console.WriteLine("\tFeatures loading step skipped. Downloading is not possible without authorization!");
}

Console.WriteLine("Features loading step is ended.");
Console.WriteLine();


// STEP THREE - get Values ======================================================
Console.WriteLine("Loading of Values step is started.");

if (_authorizationResult is not null)
{
    do
    {

        var _jsonRequest = new RequestAllValuesWithMaximumDetailsAPIv2DTO()
        {
            From = _numberOfAlreadyLoadedValues,
            Size = 1000,
            Languagecode = "EN",
            Deprecated = false,
            Include = new Include
            {
                Descriptions = true,
                Translations = false
            }
        };

        var _httpPostRequest = new HttpRequestMessage(HttpMethod.Post, _valueUri)
        {
            Content = JsonContent.Create(_jsonRequest)
        };

        _httpPostRequest.Headers.Add("Authorization", _token);

        HttpResponseMessage? _response = null;

        try
        {
            _response = _httpClient.SendAsync(_httpPostRequest).Result;
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\tError! {e.Message}");
            Console.WriteLine("\tCheck internet connection and try again!");
        }
        finally
        {
            Console.ResetColor();
        }

        ResultOfRequestAllValuesWithMaximumDetailsAPIv2DTO? _result = null;

        if (_response is not null && _response.IsSuccessStatusCode)
        {
            Console.WriteLine("\tData was loaded successful.");
            try
            {
                _result = _response.Content.ReadFromJsonAsync<ResultOfRequestAllValuesWithMaximumDetailsAPIv2DTO>().Result;
            }
            catch (NotSupportedException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\tError! Content type is not supported.");
            }
            catch (JsonException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\tError! Invalid json.");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\tError! {e.Message}");
            }
            finally
            {
                Console.ResetColor();
            }
        }
        else
        {
            Console.WriteLine("\tData loading is failed.");
            break;
        }

        if (_result is not null)
        {
            _totalNumberOfValues = _result.Total;
            _listofAllValues.AddRange(_result.Values);
            _numberOfAlreadyLoadedValues = _listofAllValues.Count;
        }
        else
        {
            break;
        }

    }
    while (_numberOfAlreadyLoadedValues < _totalNumberOfValues);

    Console.WriteLine($"\t{_totalNumberOfValues}/{_numberOfAlreadyLoadedValues} - Values (total number / loaded).");
}
else
{
    Console.WriteLine("\tValues loading step skipped. Downloading is not possible without authorization!");
}

Console.WriteLine("Values loading step is ended.");
Console.WriteLine();


// STEP FOUR - save data to file ======================================================
Console.WriteLine("Data in file saving step is started.");

// If directory does not exist, create it
if (!Directory.Exists(_pathToFilesDirectory))
{
    Directory.CreateDirectory(_pathToFilesDirectory);
}

var _xmlFileEntity = new EtimFeaturesAndValuesXmlFileEntity()
{
    Name = "ETIM Features and Values",
    CreatedBy = "Me",
    CreatedAt = DateTime.Now,
    Description = "File with actual ETIM Features and Values for offline using.",
    ContactInformation = "https://www.linkedin.com/in/oleksiiprykhodko",
    NumberOfEtimFeatures = _listOfAllFeatures.Count,
    NumberOfEtimValues = _listofAllValues.Count,
    EtimFeatures = _listOfAllFeatures.ToArray(),
    EtimValues = _listofAllValues.ToArray()
};

var _fileName = $"ETIM Features and Values {DateTime.Now.Year}.{DateTime.Now.Month}.{DateTime.Now.Day} {DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}.xml";
var _pathToXmlFile = $"{_pathToFilesDirectory}/{_fileName}";

var _xmlSerializer = new XmlSerializer(typeof(EtimFeaturesAndValuesXmlFileEntity));
try
{
    using (var _streamWriter = new StreamWriter(_pathToXmlFile))
    {
        _xmlSerializer.Serialize(_streamWriter, _xmlFileEntity);
    }
    Console.WriteLine($"\tFile '{_fileName}' was created.");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\tError! A problem occurred when creating a while file creating.");
}
finally
{
    Console.ResetColor();
}

Console.WriteLine("Data in file saving step is ended.");
Console.WriteLine();

Console.WriteLine("The End");
Console.ReadKey();




static string CreateAuthorizationString(string clientId, string clientSecret)
{
    var textBytes = System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
    var textBytesAsString = System.Convert.ToBase64String(textBytes);
    return $"Basic {textBytesAsString}";
}













