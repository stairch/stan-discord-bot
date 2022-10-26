using Discord.Commands;

namespace StanBot.Core.Commands
{
    public class Miscellaneous : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Alias("latency")]
        [Summary("Displays the bot's current latency")]
        public Task PingCommand() => ReplyAsync($"Pong! The bot's latency is {Context.Client.Latency} ms.");
    }
}
