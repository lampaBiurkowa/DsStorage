using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DsStorage.ApiClient;

public static class IServiceCollectionExtensions
{
    public static void AddDsStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DsStorageOptions>()
            .Bind(configuration.GetSection(DsStorageOptions.SECTION));

        services.AddHttpClient();
        services.AddTransient<DsStorageClientFactory>();
    }
}
