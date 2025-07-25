namespace athan.Services;

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Models;


public class LocationService(HttpClient client)
{
  private readonly HttpClient _client = client;

  public async Task<Location> FetchLocationAsync()
  {
    string jsonContent = await _client.GetStringAsync("https://ipinfo.io");
    var jsonDoc = JsonDocument.Parse(jsonContent);
    var root = jsonDoc.RootElement;

    string city = root.GetProperty("city").ToString();
    string country = root.GetProperty("country").ToString();
    string[] coords = root.GetProperty("loc").ToString().Split(',');
    double latitude = double.Parse(coords[0]);
    double longitude = double.Parse(coords[1]);

    var location = new Location(city, country, latitude, longitude);

    return location;
  }
}