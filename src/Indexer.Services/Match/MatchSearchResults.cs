namespace Indexer.Services.Match;

public class MatchSearchResults : MatchResult<MatchImage> { }

public class MatchSearchResults<T> : MatchResult<MatchMeta<T>> { }
