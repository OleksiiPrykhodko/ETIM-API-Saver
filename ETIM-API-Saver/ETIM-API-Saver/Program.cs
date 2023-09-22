using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.Json;
using System.Xml.Serialization;
using BLL.Services;
using Common.DTO;
using Common.DTO.Request;
using Common.DTO.Result;
using static System.Net.Mime.MediaTypeNames;

Console.Title = "ETIM API Saver";

string _clientId = string.Empty;
string _clientSecret = string.Empty;

const string _pathToFilesDirectory = @"./Generated files";

string _accessToken = string.Empty;

HttpClient _httpClient = new HttpClient();

ApiAuthorizationService _apiAuthorizationService = new ApiAuthorizationService(_httpClient);
ApiRequestService _apiService;

var _listOfAllFeatures = new List<Feature>();
var _listofAllValues = new List<Value>();

int _totalNumberOfFeatures = 0;
int _totalNumberOfValues = 0;

int _numberOfAlreadyLoadedFeatures = 0;
int _numberOfAlreadyLoadedValues = 0;

bool _successfulResult = false;

// STEP ONE - get access token ======================================================

Console.WriteLine();

while (_accessToken == string.Empty)
{
    Console.BackgroundColor = ConsoleColor.Gray;
    Console.ForegroundColor = ConsoleColor.Black;
    Console.WriteLine("\n Authorization \n");
    Console.ResetColor();

    Console.WriteLine("\tEnter your credentials\n");
    Console.Write("\tclient_id: ");
    _clientId = Console.ReadLine() ?? string.Empty;
    Console.WriteLine();
    Console.Write("\tclient_secret: ");
    _clientSecret = Console.ReadLine() ?? string.Empty;
    Console.WriteLine();

    try
    {
        _accessToken = _apiAuthorizationService.GetAuthorizationToken(_clientId, _clientSecret);
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
        Console.WriteLine($"\t\tError! {ex.Message}");
    }
    finally
    {
        Console.ResetColor();
    }

    if (_accessToken != string.Empty)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\t\tAuthorization successful");
        Console.ResetColor();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\t\tAuthorization is failed. Wrong ID or Password.");
        Console.ResetColor();
        Thread.Sleep(3000);
        Console.Clear();
    }
}

Console.BackgroundColor = ConsoleColor.Gray;
Console.ForegroundColor = ConsoleColor.Black;
Console.WriteLine("\n Authorization is ended ");
Console.ResetColor();
Thread.Sleep(3000);
Console.Clear();

_apiService = new ApiRequestService(_httpClient, _accessToken);

// STEP TWO - get Features ======================================================
Console.WriteLine("\n Step of Features loading is started \n");

var _jsonFeaturesRequest = new RequestAllFeatureWithMaximumDetailsAPIv2DTO()
{
    From = 0,
    Size = 1000,
    Languagecode = "EN",
    Deprecated = false,
    Include = new Include
    {
        Descriptions = true,
        Translations = false
    }
};

do
{
    _jsonFeaturesRequest.From = _numberOfAlreadyLoadedFeatures;

    ResultOfRequestAllFeatureWithMaximumDetailsAPIv2DTO _loadedFeatures =
        new ResultOfRequestAllFeatureWithMaximumDetailsAPIv2DTO()
        {
            Total = 0,
            Features = new Feature[0]
        };

    try
    {
        Console.ForegroundColor = ConsoleColor.Red;
        _loadedFeatures = _apiService.GetFeatures(_jsonFeaturesRequest);
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

        Console.WriteLine($"\t{_numberOfAlreadyLoadedFeatures} Features was loaded successful");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\tFeatures loading is failed");
        Console.ResetColor();
        break;
    }

}
while (_numberOfAlreadyLoadedFeatures < _totalNumberOfFeatures);

Console.WriteLine($"\t\t{_numberOfAlreadyLoadedFeatures}/{_totalNumberOfFeatures} - Features (loaded / total number)");

Console.WriteLine("\n Step of Features loading is ended \n");


// STEP THREE - get Values ======================================================
Console.WriteLine("\n Step of Values loading is started \n");

var _jsonValuesRequest = new RequestAllValuesWithMaximumDetailsAPIv2DTO()
{
    From = 0,
    Size = 1000,
    Languagecode = "EN",
    Deprecated = false,
    Include = new Include
    {
        Descriptions = true,
        Translations = false
    }
};

do
{
    _jsonValuesRequest.From = _numberOfAlreadyLoadedValues;

    var _loadedValues = new ResultOfRequestAllValuesWithMaximumDetailsAPIv2DTO()
    {
        Total = 0,
        Values = new Value[0]
    };

    try
    {
        Console.ForegroundColor = ConsoleColor.Red;
        _loadedValues = _apiService.GetValues(_jsonValuesRequest);
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

    if (_loadedValues.Values.Length > 0)
    {
        _totalNumberOfValues = _loadedValues.Total;
        _listofAllValues.AddRange(_loadedValues.Values);
        _numberOfAlreadyLoadedValues = _listofAllValues.Count;

        Console.WriteLine($"\t{_numberOfAlreadyLoadedValues} Values was loaded successful");
    }
    else
    {
        Console.ForegroundColor= ConsoleColor.Red;
        Console.WriteLine("\tValues loading is failed");
        Console.ResetColor();
        break;
    }
}
while (_numberOfAlreadyLoadedValues < _totalNumberOfValues);

Console.WriteLine($"\t\t{_numberOfAlreadyLoadedValues}/{_totalNumberOfValues} - Values (loaded / total number)");

Console.WriteLine("\n Step of Values loading is ended \n");


// STEP FOUR - save data to file ======================================================
Console.WriteLine("\n Step of saving data to a file is started \n");


// If directory does not exist, create it
if (!Directory.Exists(_pathToFilesDirectory))
{
    Directory.CreateDirectory(_pathToFilesDirectory);
}

if (_totalNumberOfFeatures != 0 && 
    _totalNumberOfValues != 0 &&
    _totalNumberOfFeatures == _numberOfAlreadyLoadedFeatures &&
    _totalNumberOfValues == _numberOfAlreadyLoadedValues)
{
    var _xmlFileEntity = new EtimFeaturesAndValuesXmlFileEntity()
    {
        Name = "ETIM Features and Values",
        CreatedBy = _clientId,
        CreatedAt = DateTime.Now,
        Description = "File with actual ETIM Features and Values for offline using",
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
        Console.WriteLine($"\tFile '{_fileName}' was created");
        _successfulResult = true;
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\tError! A problem occurred when creating a while file creating");
    }
    finally
    {
        Console.ResetColor();
    }
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\tStep of saving data to a file was skipped! Not all data was loaded from server");
    Console.ResetColor();
}

Console.WriteLine("\n Step of saving data to a file is ended \n");

if (_successfulResult)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(" > SUCCESS <");
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(" > FAILURE <");
}

Console.WriteLine("\n Press any key to close the window");
Console.ReadKey();


static string CreateAuthorizationString(string clientId, string clientSecret)
{
    var textBytes = System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
    var textBytesAsString = System.Convert.ToBase64String(textBytes);
    return $"Basic {textBytesAsString}";
}













