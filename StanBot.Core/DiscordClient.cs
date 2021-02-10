using System;
using System.Linq;
using System.Reflection;

namespace StanBot.Core
{
    using System.Threading.Tasks;

    using Discord;
    using Discord.WebSocket;

    public class DiscordClient
    {
        private readonly BaseSocketClient discordClient;

        public DiscordClient()
        {
            this.discordClient = new DiscordSocketClient();
        }

        public string GuildName { get; private set; }

        public string AnnouncementRoleName { get; private set; }

        public string StudentRoleName { get; private set; }

        public void Initialize(string guildName, string announcementRoleName, string studentRoleName)
        {
            this.GuildName = guildName;
            this.AnnouncementRoleName = announcementRoleName;
            this.StudentRoleName = studentRoleName;
        }

        public async Task LoginAsync(string token)
        {
            await this.discordClient.LoginAsync(TokenType.Bot, token);
            await this.discordClient.StartAsync();
        }

        public void RegisterUserJoinedListener(Func<SocketGuildUser, Task> callback)
        {
            this.discordClient.UserJoined += callback;
        }

        public void UnregisterUserJoinedListener(Func<SocketGuildUser, Task> callback)
        {
            this.discordClient.UserJoined -= callback;
        }

        public void RegisterMessageReceivedListener(Func<SocketMessage, Task> callback)
        {
            this.discordClient.MessageReceived += callback;
        }

        public void UnregisterMessageReceivedListener(Func<SocketMessage, Task> callback)
        {
            this.discordClient.MessageReceived -= callback;
        }
    }
}