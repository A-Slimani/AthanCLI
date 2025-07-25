using athan.Models;
using athan.Services;

public class AthanAppService
{
    private readonly CacheService _cacheService = new CacheService();

    public async Task<(AthanTimes, Location)> UpdateAndGetDataAsync()
    {
        _cacheService.CreateCache();

        string locationCacheFilePath = Path.Combine(_cacheService.CacheDirectory, "location.json");
        var location = _cacheService.GetCachedData<Location>(locationCacheFilePath);
        int updateFreqDays = 7;

        if (_cacheService.RefreshCheck(locationCacheFilePath, updateFreqDays))
        {
            using var client = new HttpClient();
            var locationService = new LocationService(client);
            Console.WriteLine("updating location...");
            location = await locationService.FetchLocationAsync();
            _ = _cacheService.SaveToCache(locationCacheFilePath, location);
        }

        string athanCacheFilePath = Path.Combine(_cacheService.CacheDirectory, "athan.json");
        var athanTimes = _cacheService.GetCachedData<AthanTimes>(athanCacheFilePath);
        updateFreqDays = 1;

        if (_cacheService.RefreshCheck(athanCacheFilePath, updateFreqDays))
        {
            Console.WriteLine("updating athan times...");

            using var client = new HttpClient();
            var athanService = new AthanService(client);

            var lat = location.Latitude;
            var lon = location.Longitude;
            athanTimes = await athanService.FetchAthanTimesWithCoordsAsync(lat, lon);
            _ = _cacheService.SaveToCache(athanCacheFilePath, athanTimes);
        }

        return (athanTimes, location);
    }
}