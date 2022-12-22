using Discord.Commands;
using Discord.WebSocket;
using Discord;
using NLog;

namespace StanBot.Services
{
    public class LogService
    {
        private readonly Logger _discordLogger = LogManager.GetLogger("LogService.DiscordSocketClient");
        private readonly Logger _commandServiceLogger = LogManager.GetLogger("LogService.CommandService");

        private readonly DiscordSocketClient _discordSocketClient;
        private readonly CommandService _commandService;

        public LogService(DiscordSocketClient discordSocketClient, CommandService commandService)
        {
            _discordSocketClient = discordSocketClient;
            _commandService = commandService;

            _discordSocketClient.Log += LogDiscord;
            _commandService.Log += LogCommand;
        }

        private Task LogDiscord(LogMessage message)
        {
            _discordLogger.Log(
                LogLevelFromSeverity(message.Severity),
                message.ToString(prependTimestamp: false));
            return Task.CompletedTask;
        }

        private Task LogCommand(LogMessage message)
        {
            // Return an error message for async commandService
            if (message.Exception is CommandException command)
            {
                // Don't risk blocking the logging task by awaiting a message send; ratelimits!?
                var _ = command.Context.Channel.SendMessageAsync($"Error: {command.Message}\nBitte kontaktiere einen Administrator.");
            }

            _commandServiceLogger.Log(
                LogLevelFromSeverity(message.Severity),
                message.ToString(prependTimestamp: false));
            return Task.CompletedTask;
        }

        private static LogLevel LogLevelFromSeverity(LogSeverity severity)
            => LogLevel.FromOrdinal(Math.Abs((int)severity - 5));

    }
}
