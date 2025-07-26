using System.Diagnostics.Contracts;
using System.Text.Json;
using athan.Models;

namespace athan.Services;

public class AthanService(HttpClient client)
{
  private readonly HttpClient _client = client;
  private readonly string baseApiStr = "https://api.aladhan.com/v1"; 
  private readonly string todaysDate = DateTime.Today.ToString("dd-MM-yyyy");

  public async Task<AthanTimes> FetchAthanTimesWithCoordsAsync(double latitude, double longitude)
  {
    string apiString = $"{baseApiStr}/timings/{todaysDate}?latitude={latitude}&longitude={longitude}&method=3";

    string jsonContent = await GetApiResponse(apiString); 

    AthanTimes athanTimes = ExtractAthanTimes(jsonContent);

    return athanTimes;
  }

  public async Task<AthanTimes> FetchAthanTimesWithCityAndCountry(string city, string country)
  {
    string apiString = $"{baseApiStr}/timingsByCity/{todaysDate}?city={city}&country={country}&method=3";

    string jsonContent = await GetApiResponse(apiString);

    AthanTimes athanTimes = ExtractAthanTimes(jsonContent);

    return athanTimes; 
  }

  private async Task<string> GetApiResponse(string apiStr)
  {
    var response = await _client.GetAsync(apiStr);

    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
    {
      string errorContent = await response.Content.ReadAsStringAsync();
      throw new HttpRequestException($"Bad Request (400): {errorContent}");
    }

    response.EnsureSuccessStatusCode();
    string jsonContent = await response.Content.ReadAsStringAsync();

    return jsonContent;
  }

  private static AthanTimes ExtractAthanTimes(string jsonContent)
  {
    using JsonDocument doc = JsonDocument.Parse(jsonContent);
    JsonElement prayerTimes = doc.RootElement.GetProperty("data").GetProperty("timings");

    var fajr = prayerTimes.GetProperty("Fajr").ToString();
    var sunrise = prayerTimes.GetProperty("Sunrise").ToString();
    var dhuhr = prayerTimes.GetProperty("Dhuhr").ToString();
    var asr = prayerTimes.GetProperty("Asr").ToString();
    var maghrib = prayerTimes.GetProperty("Maghrib").ToString();
    var isha = prayerTimes.GetProperty("Isha").ToString();

    return new AthanTimes(fajr, sunrise, dhuhr, asr, maghrib, isha);
  }
}
