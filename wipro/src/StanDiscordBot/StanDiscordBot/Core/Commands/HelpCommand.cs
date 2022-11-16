using Discord;
using Discord.Commands;

namespace StanBot.Core.Commands
{
    public class HelpCommand: ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commandService;

        public HelpCommand(CommandService commandService)
        {
            _commandService = commandService;
        }

        [Command("help")]
        [Alias("command", "commands")]
        [Summary("Displays all of the bot's commands, or displays info about specific command.")]
        public async Task Help([Remainder] string command = "")
        {
            string prefix = StanBotConfigLoader.Config.Prefix;
            if (command == "")
            {
                var embed = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                    Description = "These are the commands you can use."
                };

                foreach (var module in _commandService.Modules)
                {
                    string description = "";
                    foreach (var moduleCommand in module.Commands)
                    {
                        description += $"{prefix}{moduleCommand.Aliases.First()}\n";
                    }

                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        string name = module.Name;
                        embed.AddField(field =>
                        {
                            field.Name = name;
                            field.Value = description;
                            field.IsInline = false;
                        });
                    }
                }

                await ReplyAsync(embed: embed.Build());
            }
            else
            {
                var result = _commandService.Search(Context, command);

                if (!result.IsSuccess)
                {
                    await ReplyAsync($"Sorry, I couldn't find a command like **{command}**");
                    return;
                }

                var embed = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                    Description = $"Here are some commands like **{command}**"
                };

                foreach (var match in result.Commands)
                {
                    var matchingCommand = match.Command;
                    embed.AddField(field =>
                    {
                        field.Name = string.Join(", ", matchingCommand.Aliases);
                        field.Value = $"Parameter: {string.Join(", ", matchingCommand.Parameters.Select(p => p.Name))}\n" +
                        $"Summary: {matchingCommand.Summary}";
                        field.IsInline = false;
                    });
                }

                await ReplyAsync(embed: embed.Build());
            }
        }
    }
}
