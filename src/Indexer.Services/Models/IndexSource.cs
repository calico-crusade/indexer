namespace Indexer.Services.Models;

public record class IndexSource(
    string Name,
    string? UserAgent,
    string? Referer,
    int RatelimitCount,
    int RatelimitDelay);
