using athan.Models;
using athan.Services;

public class AthanAppService
{
  private readonly CacheService _cacheService;
  private readonly string locationCacheFilePath;
  private readonly string athanCacheFilePath;

  public AthanAppService()
  {
    _cacheService = new();
    locationCacheFilePath = Path.Combine(_cacheService.CacheDirectory, "location.json");
    athanCacheFilePath = Path.Combine(_cacheService.CacheDirectory, "athan.json");
  }

  public async Task<(AthanTimes, Location)> UpdateAndGetDataAsync(bool forceRefresh)
  {
    _cacheService.CreateCache();

    var location = _cacheService.GetCachedData<Location>(locationCacheFilePath);
    int locationUpdateFreqDays = 7;

    if (_cacheService.RefreshCheck(locationCacheFilePath, locationUpdateFreqDays) || forceRefresh)
    {
      using var client = new HttpClient();
      var locationService = new LocationService(client);
      Console.WriteLine("updating location...");
      location = await locationService.FetchLocationAsync();
      _ = _cacheService.SaveToCache(locationCacheFilePath, location);
      Console.WriteLine($"New set location: {location.City}, {location.Country}");
    }

    var athanTimes = _cacheService.GetCachedData<AthanTimes>(athanCacheFilePath);
    int athanUpdateFreqDays = 1;

    if (_cacheService.RefreshCheck(athanCacheFilePath, athanUpdateFreqDays) || forceRefresh)
    {
      using var client = new HttpClient();
      var athanService = new AthanService(client);

      double? lat, lon;
      if (location != null)
      {
        lat = location.Latitude;
        lon = location.Longitude;
      }
      else throw new InvalidOperationException("Missing location");

      if (lat.HasValue && lon.HasValue)
        athanTimes = await athanService.FetchAthanTimesWithCoordsAsync(lat.Value, lon.Value);
      else
        athanTimes = await athanService.FetchAthanTimesWithCityAndCountry(location.City, location.Country);

      _ = _cacheService.SaveToCache(athanCacheFilePath, athanTimes);
    }

    if (location == null) throw new InvalidOperationException("missing location values");
    if (athanTimes == null) throw new InvalidOperationException("missing athantime values");

    return (athanTimes, location);
  }

  // public void ManualOverrideLocation(string city, string country)
  // {
  //   Location overrideLocation = new(city, country);
  //   _cacheService.SaveToCache(locationCacheFilePath, overrideLocation);
  // }
}