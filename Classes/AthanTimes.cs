using System.Text.Json.Serialization;
using Spectre.Console;

namespace Athan.Classes;


public class AthanTimes
{
  public AthanTimes(string fajr, string sunrise, string dhuhr, string asr, string maghrib, string isha)
  {
    Fajr = TimeOnly.Parse(fajr);
    Sunrise = TimeOnly.Parse(sunrise);
    Dhuhr = TimeOnly.Parse(dhuhr);
    Asr = TimeOnly.Parse(asr);
    Maghrib = TimeOnly.Parse(maghrib);
    Isha = TimeOnly.Parse(isha);
  }

  [JsonConstructor]
  public AthanTimes(TimeOnly fajr, TimeOnly sunrise, TimeOnly dhuhr, TimeOnly asr, TimeOnly maghrib, TimeOnly isha)
  {
    Fajr = fajr;
    Sunrise = sunrise;
    Dhuhr = dhuhr;
    Asr = asr;
    Maghrib = maghrib;
    Isha = isha;
  }
  
  public TimeOnly Fajr { get; }
  public TimeOnly Sunrise { get; }
  public TimeOnly Dhuhr { get; }
  public TimeOnly Asr { get; }
  public TimeOnly Maghrib { get; }
  public TimeOnly Isha { get; }

  public Table AthanTable()
  {
    Table athanTable = new();

    athanTable.AddColumn("Prayer");
    athanTable.AddColumn("Time");

    athanTable.AddRow("Fajr", Fajr.ToString());
    athanTable.AddRow("Sunrise", Sunrise.ToString());
    athanTable.AddRow("Dhuhr", Dhuhr.ToString());
    athanTable.AddRow("Asr", Asr.ToString());
    athanTable.AddRow("Maghrib", Maghrib.ToString());
    athanTable.AddRow("Isha", Isha.ToString());

    return athanTable;
  }

  public string NextAthanTime()
  {
    TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);
    (string name, TimeOnly nextTime) = currentTime switch
    {
      _ when currentTime < Dhuhr => (nameof(Dhuhr), Dhuhr),
      _ when currentTime < Asr => (nameof(Asr), Asr),
      _ when currentTime < Maghrib => (nameof(Maghrib), Maghrib),
      _ when currentTime < Isha => (nameof(Isha), Isha),
      _ => (nameof(Fajr), Fajr),
    };

    TimeSpan timeDiff = nextTime.ToTimeSpan() - currentTime.ToTimeSpan();
    if (name == "Fajr")
    {
      timeDiff = nextTime.ToTimeSpan() + TimeSpan.FromDays(1) - currentTime.ToTimeSpan();
    }

    int hoursLeft = timeDiff.Hours;
    int minLeft = timeDiff.Minutes;

    string hoursStr = hoursLeft switch
    {
      > 1 => $"{hoursLeft} hours",
      1 => $"{hoursLeft} hour",
      _ => string.Empty
    };

    string minStr = minLeft switch
    {
      > 1 => $"{minLeft} minutes",
      1 => $"{minLeft} minute",
      _ => string.Empty
    };

    string combinedStr;

    if (string.IsNullOrEmpty(hoursStr))
      combinedStr = minStr;
    else if (string.IsNullOrEmpty(minStr))
      combinedStr = hoursStr;
    else
      combinedStr = $"{hoursStr} and {minStr}";

    return $"Next prayer time is {name} in {combinedStr} at {nextTime}";
  }
};