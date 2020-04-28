using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace StanBot.Core.MessageProcessors
{
    public class AnnouncementCommandMessageProcessor : IMessageProcessor
    {
        private readonly DiscordClient discordClient;
        private readonly Regex regex;

        public AnnouncementCommandMessageProcessor(DiscordClient discordClient)
        {
            this.discordClient = discordClient;
            this.ShouldContinueProcessing = false;
            this.MessageShouldTargetBot = true;
            this.AllowedMessageSources = new[] {MessageSource.User};
            this.regex = new Regex("-a (\\S*) ([\\s\\S]*)", RegexOptions.IgnoreCase);
        }

        public bool ShouldContinueProcessing { get; }

        public bool MessageShouldTargetBot { get; }

        public IEnumerable<MessageSource> AllowedMessageSources { get; }

        public bool IsMatch(SocketMessage message)
        {
            SocketGuild socketGuild = message.Author.MutualGuilds.Single(x => x.Name == this.discordClient.GuildName);
            SocketGuildUser socketGuildUser = socketGuild.GetUser(message.Author.Id);
            return this.regex.IsMatch(message.Content) && socketGuildUser.Roles.Any(x => x.Name == this.discordClient.AnnouncementRoleName);
        }

        public Task ProcessAsync(SocketMessage message)
        {
            MatchCollection matchCollection = this.regex.Matches(message.Content);
            SocketGuild guild = message.Author.MutualGuilds.Single(x => x.Name == this.discordClient.GuildName);
            string channelName = matchCollection[0].Groups[1].Value;
            SocketTextChannel socketTextChannel = guild.TextChannels.SingleOrDefault(x => x.Name == channelName);
            if (socketTextChannel != null)
            {
                return socketTextChannel.SendMessageAsync(matchCollection[0].Groups[2].Value);
            }

            return message.Author.SendMessageAsync($"Entschuldigung, aber ich konnte den Text-Channel '{channelName}' nicht finden\n\r" +
                                                   $"I'm sorry but I couldn't find the text-channel '{channelName}'");
        }
    }
}