using System.IO.Abstractions;
using Cypher.Commands;
using Spectre.Console;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config =>
{
    config.Settings.Registrar.RegisterInstance<IFileSystem>(new FileSystem());
    config.Settings.Registrar.RegisterInstance(AnsiConsole.Console);

    config.AddCommand<SetCommand>("set")
        .WithExample(new[] { "cypher", "set", "-u", "$name", "-p", "$pass", "-s", "$secret" });

    config.AddCommand<GenerateCommand>("gen")
        .WithExample(new[] { "cypher", "gen" });

    // config.AddCommand<Connect>("connect", ctx => { })
    //     .WithExample(new[] { "cypher", "connect" });

    config.SetExceptionHandler(ex =>
    {
        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);

        return -99;
    });
});

return app.Run(args);