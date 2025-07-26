using Spectre.Console;

var athanAppService = new AthanAppService();

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

if (options.ShowAll)
{
  
}

Console.WriteLine();
