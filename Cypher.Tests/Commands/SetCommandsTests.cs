using System.IO.Abstractions;
using System.IO.Abstractions.Extensions;
using System.IO.Abstractions.TestingHelpers;
using Cypher.Commands;
using FluentAssertions;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Cypher.Tests.Commands;

public class SetCommandsTests
{
    private readonly IFileSystem fileSystem = new MockFileSystem();
    private readonly string path;

    public SetCommandsTests()
    {
        path = CypherModule.GetPath();
        string directory = Path.GetDirectoryName(path);
        fileSystem.Directory.CreateDirectory(directory);
    }

    [Fact]
    public void SetCommand()
    {
        var app = new CommandAppTester();
        app.SetDefaultCommand<SetCommand>();
        app.Configure(config =>
        {
            config.Settings.Registrar.RegisterInstance(fileSystem);
            config.PropagateExceptions();
        });

        var result = app.Run(new[]
        {
            "-u",
            "John",
            "-p",
            "Password",
            "-s",
            "PEPTHLLCKWFSFJVCMX7QNHITRM2PDS3G"
        });

        result.ExitCode.Should().Be(0);
        fileSystem.CurrentDirectory()
            .File(this.path)
            .Should()
            .NotBeNull();
    }
}