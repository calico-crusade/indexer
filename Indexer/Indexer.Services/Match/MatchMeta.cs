namespace Indexer.Services.Match;

public class MatchMeta<T> : MatchImage
{
    [JsonPropertyName("metadata")]
    public T? Metadata { get; set; }
}