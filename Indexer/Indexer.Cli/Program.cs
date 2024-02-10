return await new ServiceCollection()
    .AddConfig(c =>
    {
        c.AddEnvironmentVariables()
         .AddFile("appsettings.json")
         .AddCommandLine(args);
    }, out var config)
    .AddIndexer(config)
    .Cli(args, c =>
    {
        c.Add<TestVerb>()
         .Add<IndexVerb>();
    });