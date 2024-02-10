namespace Indexer.Services;

using Match;

public static class DiExtensions
{
    public static IServiceCollection AddIndexer(this IServiceCollection services, IConfiguration config)
    {
        var userAgent = config["MangaDex:UserAgent"] ?? throw new NullReferenceException("MangaDex:UserAgent is required");

        return services
            .AddJson()
            .AddRedis()
            .AddCardboardHttp()
            .AddFileCache()
            .AddSerilog()
            .AddMangaDex("", userAgent: userAgent, throwOnError: true)
            .AddTransient<IMatchApiService, MatchApiService>()
            .AddTransient<IConfigurationService, ConfigurationService>()
            .AddTransient<IIndexQueueHandler, IndexQueueHandler>()
            .AddTransient<IFileIndexService, FileIndexService>();
    }
}
