using System.IO.Abstractions;
using System.Text.Json;
using Cypher.Infrastructure;
using Spectre.Console.Cli;

namespace Cypher.Commands;

public class SetCommand : AsyncCommand<SetCommand.Settings>
{
    private readonly IFileSystem fileSystem;

    public class Settings : CommandSettings
    {
        [CommandOption("-u|--user")]
        public string User { get; set; }

        [CommandOption("-p|--password")]
        public string Password { get; set; }

        [CommandOption("-s|--secret")]
        public string Secret { get; set; }
    }

    public SetCommand(IFileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var json = JsonSerializer.Serialize(settings);
        await this.fileSystem.WriteToFile(CypherModule.GetPath(), json);

        return 0;
    }
}