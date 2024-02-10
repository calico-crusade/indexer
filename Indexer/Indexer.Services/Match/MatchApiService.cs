﻿namespace Indexer.Services.Match;

public interface IMatchApiService
{
    Task<MatchResult?> Add<T>(string url, string id, T json);

    Task<MatchResult?> Add(string url, string id, string? json = null);

    Task<MatchResult?> Add<T>(string id, T json, byte[] file);

    Task<MatchResult?> Delete(string id);

    Task<MatchSearchResults?> Search(string url, bool allOris = false);

    Task<MatchSearchResults<T>?> Search<T>(string url, bool allOris = false);

    Task<MatchSearchResults<T>?> Search<T>(Stream io, string filename, bool allOris = false);

    Task<MatchSearchResults<T>?> Search<T>(MemoryStream io, string filename, bool allOris = false);

    Task<MatchCompareResults?> Compare(string url1, string url2);

    Task<MatchResult<int>?> Count();

    Task<MatchResult<string>?> List(int offset = 0, int limit = 20);

    Task<MatchResult?> Ping();
}

internal class MatchApiService(IApiService _api, IConfigurationService _config) : IMatchApiService
{
    public string MatchUrl => _config.Required("Match:Url");

    public Task<T?> Request<T>(string url, string method, params (string key, string value)[] body)
    {
        var req = _api.Create($"{MatchUrl}{url}", method);

        if (body != null && body.Length > 0)
            req.Body(body);

        return req.Result<T>();
    }

    public Task<MatchResult?> Add<T>(string url, string id, T json)
    {
        var meta = JsonSerializer.Serialize(json);
        return Add(url, id, meta);
    }

    public Task<MatchResult?> Add(string url, string id, string? json = null)
    {
        var pars = new List<(string, string)>
        {
            ("url", url), ("filepath", id)
        };

        if (!string.IsNullOrEmpty(json))
            pars.Add(("metadata", json));

        return Request<MatchResult>("add", "POST", pars.ToArray());
    }

    public async Task<MatchResult?> Add<T>(string id, T json, byte[] file)
    {
        using var body = new MultipartFormDataContent
        {
            { new StringContent(JsonSerializer.Serialize(json)), "metadata" },
            { new StringContent(id), "filepath" },
            { new ByteArrayContent(file), "image", "image.png" }
        };

        return await _api.Create($"{MatchUrl}add", "POST").BodyContent(body).Result<MatchResult>();
    }

    public Task<MatchResult?> Delete(string id) => Request<MatchResult>("delete", "DELETE", ("filepath", id));

    public Task<MatchSearchResults?> Search(string url, bool allOris = false) => Request<MatchSearchResults>("search", "POST", ("url", url), ("all_orientations", allOris ? "true" : "false"));

    public Task<MatchSearchResults<T>?> Search<T>(string url, bool allOris = false) => Request<MatchSearchResults<T>>("search", "POST", ("url", url), ("all_orientations", allOris ? "true" : "false"));

    public async Task<MatchSearchResults<T>?> Search<T>(Stream io, string filename, bool allOris = false)
    {
        using var ms = new MemoryStream();
        await io.CopyToAsync(ms);

        return await Search<T>(ms, filename, allOris);
    }

    public async Task<MatchSearchResults<T>?> Search<T>(MemoryStream io, string filename, bool allOris = false)
    {
        var req = _api.Create($"{MatchUrl}search", "POST");

        using var content = new MultipartFormDataContent
        {
            { new StringContent(allOris ? "true" : "false"), "all_orientations" },
            { new ByteArrayContent(io.ToArray()), "image", filename }
        };

        req.BodyContent(content);

        return await req.Result<MatchSearchResults<T>>();
    }

    public Task<MatchCompareResults?> Compare(string url1, string url2) => Request<MatchCompareResults>("compare", "POST", ("url1", url1), ("url2", url2));

    public Task<MatchResult<int>?> Count() => Request<MatchResult<int>>("count", "GET");

    public Task<MatchResult<string>?> List(int offset = 0, int limit = 20) => Request<MatchResult<string>>("list", "GET", ("offset", offset.ToString()), ("limit", limit.ToString()));

    public Task<MatchResult?> Ping() => Request<MatchResult>("ping", "GET");
}
