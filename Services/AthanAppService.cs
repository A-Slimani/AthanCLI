using athan.Models;
using athan.Services;

public class AthanAppService
{
  private readonly CacheService _cacheService = new();

  public async Task<(AthanTimes, Location)> UpdateAndGetDataAsync(bool forceRefresh)
  {
    _cacheService.CreateCache();

    string locationCacheFilePath = Path.Combine(_cacheService.CacheDirectory, "location.json");
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

    string athanCacheFilePath = Path.Combine(_cacheService.CacheDirectory, "athan.json");
    var athanTimes = _cacheService.GetCachedData<AthanTimes>(athanCacheFilePath);
    int athanUpdateFreqDays = 1;

    if (_cacheService.RefreshCheck(athanCacheFilePath, athanUpdateFreqDays) || forceRefresh)
    {
      using var client = new HttpClient();
      var athanService = new AthanService(client);

      Double lat;
      Double lon;
      if (location != null)
      {
        lat = location.Latitude;
        lon = location.Longitude;
      }
      else throw new InvalidOperationException("Missing location");

      athanTimes = await athanService.FetchAthanTimesWithCoordsAsync(lat, lon);
      _ = _cacheService.SaveToCache(athanCacheFilePath, athanTimes);
    }

    if (location == null) throw new InvalidOperationException("missing location values");
    if (athanTimes == null) throw new InvalidOperationException("missing athantime values");

    return (athanTimes, location);
  }
}