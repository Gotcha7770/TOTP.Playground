using System.IO.Abstractions;
using FluentAssertions.FileSystem;
using System.IO.Abstractions.TestingHelpers;
using Cypher.Commands;
using Cypher.Infrastructure;
using FluentAssertions;
using Spectre.Console.Cli;
using Spectre.Console.Testing;
using static FluentAssertions.FluentActions;

namespace Cypher.Tests.Commands;

public class SetCommandsTests
{
    private readonly MockFileSystem fileSystem = new MockFileSystem();
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
        var result = CreateApp().Run(
            "John",
            "Password",
            "PEPTHLLCKWFSFJVCMX7QNHITRM2PDS3G");

        result.ExitCode.Should().Be(0);
        fileSystem.Should()
            .Contain(this.path);
    }

    [Fact]
    public void SetCommand_NullOrWhitespaceOption_ValidationException()
    {
        Invoking(() => CreateApp()
                .Run("John",
                     "PEPTHLLCKWFSFJVCMX7QNHITRM2PDS3G"))
            .Should()
            .Throw<CommandRuntimeException>();

        Invoking(() => CreateApp()
                .Run("John",
                     " ",
                     "PEPTHLLCKWFSFJVCMX7QNHITRM2PDS3G"))
            .Should()
            .Throw<CommandRuntimeException>();
    }

    [Fact]
    public void SetCommand_SecretIsNotBase64_ValidationException()
    {
        Invoking(() => CreateApp()
                .Run("John",
                     "Password",
                     "otpauth://totp/user@hob?secret=PEPTHLLCKWFSFJVCMX7QNHITRM2PDS3G"))
            .Should()
            .Throw<CommandRuntimeException>();
    }

    [Fact]
    public async Task RewriteStorage()
    {
        var result = CreateApp()
            .Run("John",
                 "Password",
                 "YmlnIG5vbmcgdGVzdCBiYXNlNjQgc3RyaW5n");

        result = CreateApp()
            .Run("John",
                "Password",
                "PEPTHLLCKWFSFJVCMX7QNHITRM2PDS3G");

        result.ExitCode.Should()
            .Be(0);
        var json = await fileSystem.ReadFromFile(CypherModule.GetPath());
        json.Should()
            .Be("""{"User":"John","Password":"Password","Secret":"PEPTHLLCKWFSFJVCMX7QNHITRM2PDS3G"}""");
    }

    private CommandAppTester CreateApp()
    {
        var app = new CommandAppTester();
        app.SetDefaultCommand<SetCommand>();
        app.Configure(config =>
        {
            config.Settings.Registrar.RegisterInstance<IFileSystem>(fileSystem);
            config.PropagateExceptions();
        });

        return app;
    }
}