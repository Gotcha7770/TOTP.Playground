using System.Buffers.Text;
using System.ComponentModel;
using System.IO.Abstractions;
using System.Text.Json;
using Cypher.Infrastructure;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Cypher.Commands;

public class SetCommand : AsyncCommand<SetCommand.Settings>
{
    private readonly IFileSystem fileSystem;

    public class Settings : CommandSettings
    {
        [Description("Имя пользователя для подключения по VPN")]
        [CommandArgument(0, "<user>")]
        public string User { get; set; }

        [Description("Пароль для подключения по VPN")]
        [CommandArgument(1, "<password>")]
        public string Password { get; set; }

        [Description("Секрет выданный для подключения через аутентификатор, можно получить его из QR кода" +
                     "otpauth://totp/user@hob?secret=<вот это значение, которое нужно>")]
        [CommandArgument(2, "<secret>")]
        public string Secret { get; set; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(User))
                return ValidationResult.Error("user name argument should be set");

            if (string.IsNullOrWhiteSpace(Password))
                return ValidationResult.Error("password argument should be set");

            if (string.IsNullOrWhiteSpace(Secret))
                return ValidationResult.Error("secret argument should be set");

            if(!Base64.IsValid(Secret))
                return ValidationResult.Error("secret argument should be correct Base64 string");

            return ValidationResult.Success();
        }
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