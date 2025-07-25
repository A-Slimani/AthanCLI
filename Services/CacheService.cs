using System.Text.Json;

public class CacheService
{
  public string CacheDirectory = Path.Combine(Path.GetTempPath(), "AthanAppTemp");
  public List<string>? CachedFiles { get; set; }

  public void CreateCache()
  {
    if (string.IsNullOrWhiteSpace(CacheDirectory))
      throw new InvalidOperationException("Cache directory path is not set.");

    Directory.CreateDirectory(CacheDirectory);
  }

  public bool RefreshCheck(string fileName, int daysToRefresh)
  {
    string filePath = Path.Combine(CacheDirectory, fileName);
    bool fileExists = File.Exists(filePath);
    bool toRefresh = File.GetLastWriteTime(filePath).AddDays(daysToRefresh) < DateTime.Now;

    if (!fileExists || toRefresh) return true;

    return false;
  }

  public async Task SaveToCache<T>(string fileName, T content)
  {
    string convertedContent = JsonSerializer.Serialize(content);
    string filePath = Path.Combine(CacheDirectory, fileName);

    await File.WriteAllTextAsync(filePath, convertedContent);
  }

  public T? GetCachedData<T>(string fileName)
  {
    string filePath = Path.Combine(CacheDirectory, fileName);

    if (File.Exists(filePath))
    {
      using var fileStream = File.OpenRead(filePath);
      var result = JsonSerializer.Deserialize<T>(fileStream);
      return result;
    }

    return default;
  }
}