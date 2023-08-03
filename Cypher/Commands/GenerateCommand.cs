using System.IO.Abstractions;
using System.Text.Json;
using Cypher.Infrastructure;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Cypher.Commands;

public class GenerateCommand : AsyncCommand
{
    private readonly IFileSystem fileSystem;
    private readonly IAnsiConsole ansiConsole;

    public GenerateCommand(IFileSystem fileSystem, IAnsiConsole ansiConsole)
    {
        this.fileSystem = fileSystem;
        this.ansiConsole = ansiConsole;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var json = await this.fileSystem.ReadFromFile(CypherModule.GetPath());
        var settings = JsonSerializer.Deserialize<SetCommand.Settings>(json);
        var code = CypherModule.GetTotpCode(settings.Secret);

        this.ansiConsole.Markup(settings.Password + code);

        return 0;
    }
}