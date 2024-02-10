namespace Indexer.Services;

using Models;

/// <summary>
/// A helper service for interacting with the application's configuration.
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Fetches an optional configuration value.
    /// </summary>
    /// <param name="key">The configuration key</param>
    /// <returns></returns>
    string? Optional(string key);

    /// <summary>
    /// Fetches an optional int configuration value
    /// </summary>
    /// <param name="key">The configuration key</param>
    /// <returns></returns>
    int? OptionalInt(string key);

    /// <summary>
    /// Fetches a required configuration value.
    /// </summary>
    /// <param name="key">The configuration key</param>
    /// <returns></returns>
    string Required(string key);

    /// <summary>
    /// Fetches a required int configuration value.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int RequiredInt(string key);

    /// <summary>
    /// Get the configuration for the given index source
    /// </summary>
    /// <param name="name">The name of the source</param>
    /// <returns>The source configuration</returns>
    IndexSource? Source(string name);

    /// <summary>
    /// Get the configuration for the given index source
    /// </summary>
    /// <param name="name">The name of the source</param>
    /// <returns>The source configuration</returns>
    IndexSource RequiredSource(string name);
}

internal class ConfigurationService(IConfiguration _config) : IConfigurationService
{
    public string? Optional(string key) => _config[key];

    public int? OptionalInt(string key) => int.TryParse(Optional(key), out var result) ? result : null;

    public string Required(string key)
    {
        return Optional(key) ?? throw new NullReferenceException($"Configuration option required by not specified: {key}");
    }

    public int RequiredInt(string key)
    {
        return OptionalInt(key) ?? throw new NullReferenceException($"Configuration option required by not specified or invalid: {key}");
    }

    public IndexSource? Source(string name)
    {
        var section = _config.GetSection(name);
        if (section is null) return null;

        return new(name,
            Optional($"{name}:UserAgent"),
            Optional($"{name}:Referer"),
            OptionalInt($"{name}:Ratelimit:Count") ?? 0,
            (OptionalInt($"{name}:Ratelimit:DelaySec") ?? 0) * 1000);
    }

    public IndexSource RequiredSource(string name)
    {
        return Source(name) ?? throw new NullReferenceException($"Configuration section required by not specified: {name}");
    }
}