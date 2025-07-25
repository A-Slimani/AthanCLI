using System.Diagnostics.Contracts;
using System.Text.Json;
using athan.Models;

namespace athan.Services;

public class AthanService(HttpClient client)
{
  private readonly HttpClient _client = client;

  public async Task<AthanTimes> FetchAthanTimesWithCoordsAsync(double latitude, double longitude)
  {
    DateTime date = DateTime.Today;
    string formattedDate = date.ToString("dd-MM-yyyy");
    string apiString = $"https://api.aladhan.com/v1/timings/{formattedDate}?latitude={latitude}&longitude={longitude}&method=3";

    var response = await _client.GetAsync(apiString);

    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
    {
      string errorContent = await response.Content.ReadAsStringAsync();
      throw new HttpRequestException($"Bad Request (400): {errorContent}");
    }

    response.EnsureSuccessStatusCode();
    string jsonContent = await response.Content.ReadAsStringAsync();

    return ExtractAthanTimes(jsonContent);
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
