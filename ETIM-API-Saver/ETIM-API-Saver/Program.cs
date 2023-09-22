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
int _totalNumberOfNotDeprecatedFeatures = 0;
int _totalNumberOfDeprecatedFeatures = 0;
int _numberOfLoadedNotDeprecatedFeatures = 0;
int _numberOfLoadedDeprecatedFeatures = 0;
bool _continueFeaturesLoading = true;

var _listOfAllValues = new List<Value>();
int _totalNumberOfNotDeprecatedValues = 0;
int _totalNumberOfDeprecatedValues = 0;
int _numberOfLoadedNotDeprecatedValues = 0;
int _numberOfLoadedDeprecatedValues = 0;
bool _continueValuesLoading = true;

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
    _jsonFeaturesRequest.From =
        _jsonFeaturesRequest.Deprecated ? _numberOfLoadedDeprecatedFeatures : _numberOfLoadedNotDeprecatedFeatures;

    var _loadedFeatures = new ResultOfRequestAllFeatureWithMaximumDetailsAPIv2DTO()
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
        if (_jsonFeaturesRequest.Deprecated)
        {
            _totalNumberOfDeprecatedFeatures = _loadedFeatures.Total;
            _numberOfLoadedDeprecatedFeatures += _loadedFeatures.Features.Count();
        }
        else
        {
            _totalNumberOfNotDeprecatedFeatures = _loadedFeatures.Total;
            _numberOfLoadedNotDeprecatedFeatures += _loadedFeatures.Features.Count();
        }

        _listOfAllFeatures.AddRange(_loadedFeatures.Features);

        Console.WriteLine($"\t{_listOfAllFeatures.Count} Features was loaded successful");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\tFeatures loading is failed");
        Console.ResetColor();
        break;
    }

    // Switch to deprecated features loading.
    if (!_jsonFeaturesRequest.Deprecated && _numberOfLoadedNotDeprecatedFeatures == _totalNumberOfNotDeprecatedFeatures)
    {
        _jsonFeaturesRequest.Deprecated = true;
    }

    if (_totalNumberOfNotDeprecatedFeatures != 0 &&
        _totalNumberOfDeprecatedFeatures != 0 &&
        _totalNumberOfNotDeprecatedFeatures == _numberOfLoadedNotDeprecatedFeatures &&
        _totalNumberOfDeprecatedFeatures == _numberOfLoadedDeprecatedFeatures)
    {
        _continueFeaturesLoading = false;
    }
}
while (_continueFeaturesLoading);

Console.WriteLine($"\t\t{_numberOfLoadedNotDeprecatedFeatures}/{_totalNumberOfNotDeprecatedFeatures} - Number of not deprecated Features (loaded / total number)");
Console.WriteLine($"\t\t{_numberOfLoadedDeprecatedFeatures}/{_totalNumberOfDeprecatedFeatures} - Number of deprecated Features (loaded / total number)");
Console.WriteLine($"\t\t{_numberOfLoadedNotDeprecatedFeatures + _numberOfLoadedDeprecatedFeatures}/{_totalNumberOfNotDeprecatedFeatures + _totalNumberOfDeprecatedFeatures} - Features (loaded / total number)");

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
    _jsonValuesRequest.From =
        _jsonValuesRequest.Deprecated ? _numberOfLoadedDeprecatedValues : _numberOfLoadedNotDeprecatedValues;

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
        if (_jsonValuesRequest.Deprecated)
        {
            _totalNumberOfDeprecatedValues = _loadedValues.Total;
            _numberOfLoadedDeprecatedValues += _loadedValues.Values.Count();
        }
        else
        {
            _totalNumberOfNotDeprecatedValues = _loadedValues.Total;
            _numberOfLoadedNotDeprecatedValues += _loadedValues.Values.Count();
        }

        _listOfAllValues.AddRange(_loadedValues.Values);

        Console.WriteLine($"\t{_listOfAllValues.Count} Values was loaded successful");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\tValues loading is failed");
        Console.ResetColor();
        break;
    }

    // Switch to deprecated values loading.
    if (!_jsonValuesRequest.Deprecated && _numberOfLoadedNotDeprecatedValues == _totalNumberOfNotDeprecatedValues)
    {
        _jsonValuesRequest.Deprecated = true;
    }

    if (_totalNumberOfNotDeprecatedValues != 0 &&
        _totalNumberOfDeprecatedValues != 0 &&
        _totalNumberOfNotDeprecatedValues == _numberOfLoadedNotDeprecatedValues &&
        _totalNumberOfDeprecatedValues == _numberOfLoadedDeprecatedValues)
    {
        _continueValuesLoading = false;
    }
}
while (_continueValuesLoading);

Console.WriteLine($"\t\t{_numberOfLoadedNotDeprecatedValues}/{_totalNumberOfNotDeprecatedValues} - Number of not deprecated Values (loaded / total number)");
Console.WriteLine($"\t\t{_numberOfLoadedDeprecatedValues}/{_totalNumberOfDeprecatedValues} - Number of deprecated Values (loaded / total number)");
Console.WriteLine($"\t\t{_numberOfLoadedNotDeprecatedValues + _numberOfLoadedDeprecatedValues}/{_totalNumberOfNotDeprecatedValues + _totalNumberOfDeprecatedValues} - Values (loaded / total number)");

Console.WriteLine("\n Step of Values loading is ended \n");


// STEP FOUR - save data to file ======================================================
Console.WriteLine("\n Step of saving data to a file is started \n");


// If directory does not exist, create it
if (!Directory.Exists(_pathToFilesDirectory))
{
    Directory.CreateDirectory(_pathToFilesDirectory);
}

if (_totalNumberOfNotDeprecatedFeatures != 0 &&
    _totalNumberOfDeprecatedFeatures != 0 &&
    _totalNumberOfNotDeprecatedFeatures == _numberOfLoadedNotDeprecatedFeatures &&
    _totalNumberOfDeprecatedFeatures == _numberOfLoadedDeprecatedFeatures &&
    _totalNumberOfNotDeprecatedValues != 0 &&
    _totalNumberOfDeprecatedValues != 0 &&
    _totalNumberOfNotDeprecatedValues == _numberOfLoadedNotDeprecatedValues &&
    _totalNumberOfDeprecatedValues == _numberOfLoadedDeprecatedValues)
{
    var _xmlFileEntity = new EtimFeaturesAndValuesXmlFileEntity()
    {
        Name = "ETIM Features and Values",
        CreatedBy = _clientId,
        CreatedAt = DateTime.Now,
        Description = "File with actual ETIM Features and Values for offline using",
        ContactInformation = "https://www.linkedin.com/in/oleksiiprykhodko",
        NumberOfEtimFeatures = _listOfAllFeatures.Count,
        NumberOfEtimValues = _listOfAllValues.Count,
        EtimFeatures = _listOfAllFeatures.ToArray(),
        EtimValues = _listOfAllValues.ToArray()
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













