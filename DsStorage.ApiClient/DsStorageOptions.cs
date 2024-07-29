namespace DsStorage.ApiClient;

public class DsStorageOptions
{
    public const string SECTION = nameof(DsStorage);
    public required string Url { get; set; }
}