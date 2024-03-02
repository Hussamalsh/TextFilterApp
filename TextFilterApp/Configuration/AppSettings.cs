namespace TextFilterApp.Configuration;

public class AppSettings
{
    public string FilePath { get; set; }
    public FilterSettings Filters { get; set; }
    public int ChunkSize { get; set; } = 100;
}

public class FilterSettings
{
    public bool EnableVowelMiddleFilter { get; set; }
    public bool EnableMinLengthFilter { get; set; }
    public bool EnableContainsTFilter { get; set; }
}