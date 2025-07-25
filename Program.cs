using Spectre.Console;

var athanAppService = new AthanAppService();
var (athanTimes, location) = await athanAppService.UpdateAndGetDataAsync();

Console.WriteLine("===========");
if (args.Length > 0)
{
  if (args[0] == "--all")
  {
    Console.WriteLine(location.LocationString());
    AnsiConsole.Write(athanTimes.ToTable());
  }
}
else
{
  Console.WriteLine(athanTimes.NextAthanTimeStr());
}
Console.WriteLine("===========");

