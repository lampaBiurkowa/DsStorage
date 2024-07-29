using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace DsStorage.ApiClient;

public class DsStorageClientFactory(IHttpClientFactory httpClientFactory, IOptions<DsStorageOptions> options)
{
    readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    readonly DsStorageOptions options = options.Value;

    public DsStorageClient CreateClient(string bearerToken = "")
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new ($"{options.Url.TrimEnd('/')}/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        return new DsStorageClient(client);
    }
}
