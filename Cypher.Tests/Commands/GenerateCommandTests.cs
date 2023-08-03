using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Cypher.Commands;
using Cypher.Infrastructure;
using FluentAssertions;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Cypher.Tests.Commands;

public class GenerateCommandTests
{
    private readonly IFileSystem fileSystem = new MockFileSystem();
    private readonly IAnsiConsole ansiConsole = new TestConsole();
    private readonly string path;

    public GenerateCommandTests()
    {
        path = CypherModule.GetPath();
        string directory = Path.GetDirectoryName(path);
        fileSystem.Directory.CreateDirectory(directory);
    }

    [Fact]
    public async Task GenerateCommand()
    {
        await StoreSettings(new SetCommand.Settings
        {
            User = "John",
            Password = "Password",
            Secret = "PEPTHLLCKWFSFJVCMX7QNHITRM2PDS3G"
        });

        var app = new CommandAppTester();
        app.SetDefaultCommand<GenerateCommand>();
        app.Configure(config =>
        {
            config.Settings.Registrar.RegisterInstance(fileSystem);
            config.Settings.Registrar.RegisterInstance(ansiConsole);
            config.PropagateExceptions();
        });

        var result = app.Run();

        result.ExitCode.Should().Be(0);
    }

    private async Task StoreSettings(SetCommand.Settings settings)
    {
        var json = JsonSerializer.Serialize(settings);
        await fileSystem.WriteToFile(path, json);
    }
}