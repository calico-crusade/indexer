namespace Indexer.Cli.Verbs;

[Verb("index", isDefault: true, HelpText = "Read index queue and process images.")]
internal class IndexVerbOptions { }

internal class IndexVerb(
    ILogger<IndexVerb> logger,
    IIndexQueueHandler _queue) : BooleanVerb<IndexVerbOptions>(logger)
{
    public override async Task<bool> Execute(IndexVerbOptions options, CancellationToken token)
    {
        var errored = false;
        try
        {
            _logger.LogInformation("Starting index queue.");
            await _queue.Setup(token);
            await Task.Delay(-1, token);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Index queue cancelled. Was exit requested?");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Index queue failed.");
            errored = true;
        }
        finally
        {
            _queue.Dispose();
        }

        return !errored;
    }
}
