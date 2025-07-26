using Spectre.Console;

namespace athan.Models;

public class AthanTimes
{
  public TimeOnly Fajr { get; set; }
  public TimeOnly Sunrise { get; set; }
  public TimeOnly Dhuhr { get; set; }
  public TimeOnly Asr { get; set; }
  public TimeOnly Maghrib { get; set; }
  public TimeOnly Isha { get; set; }

  public AthanTimes(TimeOnly fajr, TimeOnly sunrise, TimeOnly dhuhr, TimeOnly asr, TimeOnly maghrib, TimeOnly isha)
  {
    Fajr = fajr;
    Sunrise = sunrise;
    Dhuhr = dhuhr;
    Asr = asr;
    Maghrib = maghrib;
    Isha = isha;
  }

  public AthanTimes(string fajr, string sunrise, string dhuhr, string asr, string maghrib, string isha)
  {
    Fajr = TimeOnly.Parse(fajr);
    Sunrise = TimeOnly.Parse(sunrise);
    Dhuhr = TimeOnly.Parse(dhuhr);
    Asr = TimeOnly.Parse(asr);
    Maghrib = TimeOnly.Parse(maghrib);
    Isha = TimeOnly.Parse(isha);
  }

  public AthanTimes()
  {
    Fajr = TimeOnly.MinValue;
    Sunrise = TimeOnly.MinValue;
    Dhuhr = TimeOnly.MinValue;
    Asr = TimeOnly.MinValue;
    Maghrib = TimeOnly.MinValue;
    Isha = TimeOnly.MinValue;
  }

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
    var (name, nextTime) = currentTime switch
    {
      var t when t < Dhuhr => (nameof(Dhuhr), Dhuhr),
      var t when t < Asr => (nameof(Asr), Asr),
      var t when t < Maghrib => (nameof(Maghrib), Maghrib),
      var t when t < Isha => (nameof(Isha), Isha),
      _ => (nameof(Fajr), Fajr),
    };

    var timeDiff = nextTime.ToTimeSpan() - currentTime.ToTimeSpan();
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