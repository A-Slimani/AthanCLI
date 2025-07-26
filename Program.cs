using System.Security.Authentication;
using Spectre.Console;

AthanAppService athanAppService = new();

Console.WriteLine();
// if (args.Length > 0)
// {
// 
// 
//   var (athanTimes, location) = await athanAppService.UpdateAndGetDataAsync(forceRefresh);
// 
//   if (showAll)
//   {
//     Console.WriteLine(location.LocationString());
//     AnsiConsole.Write(athanTimes.AthanTable());
//   }
//   else
//   {
//     Console.WriteLine(athanTimes.NextAthanTime());
//   }
// }
// else
// {
//   var (athanTimes, _) = await athanAppService.UpdateAndGetDataAsync(false);
//   Console.WriteLine(athanTimes.NextAthanTime());
// }

var options = AthanOptions.FromArgs(args);

if (options.SetManualLocation)
{
  if (options.LocationStr == null)
  {
    Console.WriteLine("Enter the desired location after the --set-location flag in this format \"City, Country\"");
  }
  else
  {
    string[] OverrideLocation = options.LocationStr.Split(',');
    if (OverrideLocation.Length == 2)
    {
      string city = OverrideLocation[0];
      string country = OverrideLocation[1];
      // athanAppService.ManualOverrideLocation(city, country);
    }
    else throw new InvalidOperationException("Invalid city / country format. Correct format -> \"City, Country\" "); 
  }
}

var (athanTimes, location) = await athanAppService.UpdateAndGetDataAsync(options.ForceRefresh);

Console.WriteLine();
