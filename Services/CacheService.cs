using System.Text.Json;

namespace Athan.Services;

public class CacheService
{
  private readonly string _cacheDirectory = Path.Combine(Path.GetTempPath(), "AthanAppTemp");

  public void CreateCache()
  {
    if (string.IsNullOrWhiteSpace(_cacheDirectory))
      throw new InvalidOperationException("Cache directory path is not set.");

    Directory.CreateDirectory(_cacheDirectory);
  }

  public bool RefreshCheck(string fileName, int daysToRefresh)
  {
    string filePath = Path.Combine(_cacheDirectory, fileName);
    bool fileExists = File.Exists(filePath);
    bool toRefresh = File.GetLastWriteTime(filePath).AddDays(daysToRefresh) < DateTime.Today;

    return !fileExists || toRefresh;
  }

  public Task SaveToCache<T>(T content, string fileName)
  {
    string convertedContent = JsonSerializer.Serialize(content);
    string filePath = Path.Combine(_cacheDirectory, fileName);

    return File.WriteAllTextAsync(filePath, convertedContent);
  }

  public T GetCachedData<T>(string fileName)
  {
    string filePath = Path.Combine(_cacheDirectory, fileName);

    if (!File.Exists(filePath)) throw new FileNotFoundException();
    
    using FileStream fileStream = File.OpenRead(filePath);
    
    T? result = JsonSerializer.Deserialize<T>(fileStream);
    
    if (result == null) throw new InvalidOperationException("Failed to deserialize file.");
    
    return result;
  }
}