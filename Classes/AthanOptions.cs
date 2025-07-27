namespace Athan.Classes;

public class AthanOptions
{
  public bool ShowAll { get; private set; }
  public bool ForceRefreshAthan { get; private set; }
  public bool ForceRefreshLocation { get; private set; }
  public bool SetManualLocation { get; private set; }
  public string? LocationStr { get; private set; }

  public static AthanOptions FromArgs(string[] args) => new()
  {
    ShowAll = args.Contains("--all"),
    ForceRefreshAthan = args.Contains("--force-refresh-athan"),
    ForceRefreshLocation = args.Contains("--force-refresh-location"),
    SetManualLocation = args.Contains("--set-location"),
    LocationStr = GetPassedThroughLocation(args)
  };

  private static string? GetPassedThroughLocation(string[] args)
  {
    if (!args.Contains("--set-location")) return null;
    int index = Array.IndexOf(args, "--set-location");
    try
    {
      string locationStr = args[index + 1];
      return locationStr;
    }
    catch
    {
      Console.WriteLine("Invalid or missing location string");
    }
    return null;
  }
}