namespace Indexer.Cli.Verbs;

[Verb("test", HelpText = "Testing the index posting call")]
internal class TestVerbOptions { }

internal class TestVerb(
    ILogger<TestVerb> logger,
    IFileIndexService _indexer,
    IConfigurationService _config) : BooleanVerb<TestVerbOptions>(logger)
{
    public override async Task<bool> Execute(TestVerbOptions options, CancellationToken token)
    {
        var source = _config.RequiredSource("Mangadex");

        var mangaId = "be282a1e-5a13-4f89-9d98-7da56d5dbb1e";
        var chapterId = "a54c491c-8e4c-4e97-8873-5b79e59da210";
        var url = "https://uploads.mangadex.org/data/3303dd03ac8d27452cce3f2a882e94b2/1-f7a76de10d346de7ba01786762ebbedc666b412ad0d4b73baa330a2a392dbcdd.png";

        var result = await _indexer.Index(url, source, new IndexRequest
        {
            MangaId = mangaId,
            ChapterId = chapterId,
            Type = IndexRequest.TYPE_PAGES
        }, 0);
        _logger.LogInformation("Index result: {Result}", result);
        return result;
    }
}
