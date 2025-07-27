using Athan.Classes;

namespace athan.Services;

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public abstract class LocationService()
{
  public static async Task<Location> FetchLocationAsync(HttpClient client)
  {
    string jsonContent = await client.GetStringAsync("https://ipinfo.io");
    JsonDocument jsonDoc = JsonDocument.Parse(jsonContent);
    JsonElement root = jsonDoc.RootElement;

    string city = root.GetProperty("city").ToString();
    string country = root.GetProperty("country").ToString();
    string[] coords = root.GetProperty("loc").ToString().Split(',');
    double latitude = double.Parse(coords[0]);
    double longitude = double.Parse(coords[1]);

    Location location = new (city, country, latitude, longitude);

    return location;
  }

  public static Location ManualSetLocation(string argString)
  { 
    string[] locationStr =  argString.Split(',');
    
    if (locationStr.Length != 2) throw new Exception("Invalid location string");
    
    string city = locationStr[0].Trim();  
    string country = locationStr[1].Trim();
    
    return new Location(city, country);
  }
}