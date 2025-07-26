using Spectre.Console;

public class AthanOptions
{
    public bool ShowAll { get; set; }
    public bool ForceRefresh { get; set; }
    public bool SetManualLocation { get; set; }

    public static AthanOptions FromArgs(string[] args) => new AthanOptions
    {
        ShowAll = args.Contains("--all"),
        ForceRefresh = args.Contains("--force-refresh"),
        SetManualLocation = args.Contains("--set-location")
    };
}