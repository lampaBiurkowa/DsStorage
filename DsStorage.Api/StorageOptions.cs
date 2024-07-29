namespace DsStorage.Api;

public class StorageOptions
{
    public const string SECTION = "Storage";
    public required string Key { get; set; }
    public required string Url { get; set; }
    public required string DefaultBucket { get; set; }
}