using Spectre.Console;

public class AthanOptions
{
  public bool ShowAll { get; set; }
  public bool ForceRefresh { get; set; }
  public bool SetManualLocation { get; set; }
  public string? LocationStr { get; set; }

  public static AthanOptions FromArgs(string[] args) => new()
  {
    ShowAll = args.Contains("--all"),
    ForceRefresh = args.Contains("--force-refresh"),
    SetManualLocation = args.Contains("--set-location"),
    LocationStr = GetPassedThroughLocation(args)
  };

  public static string? GetPassedThroughLocation(string[] args)
  {
    if (args.Contains("--set-location"))
    {
      int index = Array.IndexOf(args, "--set-location");
      return args[index + 1];
    }
    return default;
  }

}