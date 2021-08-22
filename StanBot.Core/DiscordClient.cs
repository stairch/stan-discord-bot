using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace StanBot.Core
{
    public class DiscordClient
    {
        private readonly BaseSocketClient discordClient;

        public string GuildName { get; private set; }

        public string AnnouncementRoleName { get; private set; }

        public string StudentRoleName { get; private set; }

        public DiscordClient()
        {
            this.discordClient = new DiscordSocketClient();
        }

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