using System.Net;
using Athan.Classes;
using athan.Services;
using Athan.Services;
using Spectre.Console;

const string locationCacheFile = "location.json";
const string athanTimesCacheFile = "athan.json";
const int locationDaysToRefresh = 7;
const int athantimesDaysToRefresh = 1;

HttpClient client = new();
CacheService cacheService = new();
cacheService.CreateCache();

bool locationRefresh = cacheService.RefreshCheck(locationCacheFile, locationDaysToRefresh);
bool athanRefresh = cacheService.RefreshCheck(athanTimesCacheFile, athantimesDaysToRefresh);

AthanTimes athanTimes;
Location location;

AthanOptions options = AthanOptions.FromArgs(args);

// GENERATING LOCATION SECTION
if (options.SetManualLocation)
{
  if (options.ForceRefreshLocation)
  {
    Console.WriteLine("Cannot use --force-refresh-location with --set-location together");
    Environment.Exit(0);
  }
  else if (options.LocationStr == null)
  {
    Console.WriteLine("Enter the desired location after the --set-location flag in this format \"City, Country\"");
    Environment.Exit(0);
  }
  else
  {
    try
    {
      location = LocationService.ManualSetLocation(options.LocationStr);
      await cacheService.SaveToCache(location, locationCacheFile);
    }
    catch (Exception e)
    {
      Console.WriteLine(e.Message);
      Environment.Exit(0);
    }
  }
}
else if (options.ForceRefreshLocation || locationRefresh)
{
  location = await LocationService.FetchLocationAsync(client);
  await cacheService.SaveToCache(location, locationCacheFile);
}

// GENERATING ATHAN SECTION
if (options.ForceRefreshAthan || athanRefresh)
{
  try
  {
    location = cacheService.GetCachedData<Location>(locationCacheFile);
    if (location.Latitude != null && location.Longitude != null)
    {
      double latitude = location.Latitude.Value; 
      double longitude = location.Longitude.Value; 
      athanTimes = await AthanService.FetchAthanTimesWithCoordsAsync(client, latitude, longitude);
    }
    else
    {
      string city = location.City;  
      string country = location.Country;
      athanTimes = await AthanService.FetchAthanTimesWithCityAndCountry(client, city, country);
    }

    await cacheService.SaveToCache(athanTimes, athanTimesCacheFile);
  }
  catch (Exception e) 
  {
    Console.WriteLine(e.Message);
  }
}

// FETCH DATA
location = cacheService.GetCachedData<Location>(locationCacheFile);
athanTimes = cacheService.GetCachedData<AthanTimes>(athanTimesCacheFile);

// DISPLAY OUTPUT 
if (options.ShowAll)
{
  AnsiConsole.WriteLine(location.LocationString());
  AnsiConsole.Write(athanTimes.AthanTable());
}
else AnsiConsole.Write(athanTimes.NextAthanTime());

Console.WriteLine();
