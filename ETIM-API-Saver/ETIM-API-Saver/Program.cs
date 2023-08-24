using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.Json;
using System.Xml.Serialization;
using Common.DTO;
using Common.DTO.Request;
using Common.DTO.Result;


Console.WriteLine("Hello, I am ETIM API Saver!");
Console.WriteLine();

const string _authorizationUri = "https://etimauth.etim-international.com/connect/token";
const string _featureUri = "https://etimapi.etim-international.com/api/v2/Feature/Search";
const string _valueUri = "https://etimapi.etim-international.com/api/v2/Value/Search";

const string _pathToFilesDirectory = @"./Generated files";

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

ResultOfRequestAccessTokenDTO? _authorizationResult = null;

if (_authorizationResponse is not null && _authorizationResponse.IsSuccessStatusCode)
{
    Console.WriteLine("Authorization successful.");
    try
    {
        _authorizationResult = _authorizationResponse.Content.ReadFromJsonAsync<ResultOfRequestAccessTokenDTO>().Result;
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
    EtimFeatures = _listOfAllFeatures.ToArray()
};

var _fileName = $"ETIM Features and Values {DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year} {DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}.xml";
var _pathToXmlFile = $"{_pathToFilesDirectory}/{_fileName}";

var _xmlSerializer = new XmlSerializer(typeof(EtimFeaturesAndValuesXmlFileEntity));
try
{
    using (var _streamWriter = new StreamWriter(_pathToXmlFile))
    {
        _xmlSerializer.Serialize(_streamWriter, _xmlFileEntity);
    }
    Console.WriteLine($"File '{_fileName}' was created.");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Error! A problem occurred when creating a while file creating.");
}
finally
{
    Console.ResetColor();
}

Console.WriteLine("Data in file saving step is ended.");
Console.WriteLine();

Console.WriteLine("The End");
Console.ReadKey();



















