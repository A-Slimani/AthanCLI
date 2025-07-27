using System.Diagnostics.Contracts;
using System.Text.Json;
using Athan.Classes;

namespace athan.Services;

public abstract class AthanService()
{
  private const string BaseApiStr = "https://api.aladhan.com/v1";
  private static readonly string TodaysDate = DateTime.Today.ToString("dd-MM-yyyy");

  public static async Task<AthanTimes> FetchAthanTimesWithCoordsAsync(HttpClient client, double latitude, double longitude)
  {
    string apiString = $"{BaseApiStr}/timings/{TodaysDate}?latitude={latitude}&longitude={longitude}&method=3";
    string jsonContent = await GetApiResponse(client, apiString); 
    
    AthanTimes athanTimes = ExtractAthanTimes(jsonContent);

    return athanTimes;
  }

  public static async Task<AthanTimes> FetchAthanTimesWithCityAndCountry(HttpClient client, string city, string country)
  {
    string apiString = $"{BaseApiStr}/timingsByCity/{TodaysDate}?city={city}&country={country}&method=3";
    string jsonContent = await GetApiResponse(client, apiString);

    AthanTimes athanTimes = ExtractAthanTimes(jsonContent);

    return athanTimes; 
  }

  private static async Task<string> GetApiResponse(HttpClient client, string apiStr)
  {
    HttpResponseMessage response = await client.GetAsync(apiStr);

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

    string fajr = prayerTimes.GetProperty("Fajr").ToString();
    string sunrise = prayerTimes.GetProperty("Sunrise").ToString();
    string dhuhr = prayerTimes.GetProperty("Dhuhr").ToString();
    string asr = prayerTimes.GetProperty("Asr").ToString();
    string maghrib = prayerTimes.GetProperty("Maghrib").ToString();
    string isha = prayerTimes.GetProperty("Isha").ToString();

    return new AthanTimes(fajr, sunrise, dhuhr, asr, maghrib, isha);
  }
}
