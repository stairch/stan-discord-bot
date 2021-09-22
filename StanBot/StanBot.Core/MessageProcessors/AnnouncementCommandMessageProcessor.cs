using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StanBot.Core.MessageProcessors
{
    public class AnnouncementCommandMessageProcessor : IMessageProcessor
    {
        private readonly DiscordClient discordClient;
        private readonly Regex regex;

        public bool ShouldContinueProcessing { get; }

        public bool MessageShouldTargetBot { get; }

        public IEnumerable<MessageSource> AllowedMessageSources { get; }

        public AnnouncementCommandMessageProcessor(DiscordClient discordClient)
        {
            this.discordClient = discordClient;
            this.ShouldContinueProcessing = false;
            this.MessageShouldTargetBot = true;
            this.AllowedMessageSources = new[] { MessageSource.User };
            // the regex is build as such:
            // -a is the identifier of the command
            // then comes the channel where it should be announced
            // then comes the message/announcement
            this.regex = new Regex("-a (\\S*) ([\\s\\S]*)", RegexOptions.IgnoreCase);
        }

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
            else
            {
                // channel not found
                // TODO: log this
                return message.Author.SendMessageAsync($"Entschuldigung, aber ich konnte den Text-Channel '{channelName}' nicht finden\n\r" +
                                                   $"I'm sorry but I couldn't find the text-channel '{channelName}'");
            }
        }
    }
}