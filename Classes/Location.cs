using System.Text.Json.Serialization;

namespace Athan.Classes;

public class Location
{
  public string City { get; set; }
  public string Country { get; set; }
  public double? Latitude { get; set; }
  public double? Longitude { get; set; }
  

  public Location(string city, string country, double latitude, double longitude)
  {
    City = city;
    Country = country;
    Latitude = latitude;
    Longitude = longitude;
  }
  
  [JsonConstructor]
  public Location(string city, string country)
  {
    City = city;
    Country = country;
  }

  public string LocationString() => $"Prayer times for {City}, {Country}";
}



