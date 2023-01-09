using Discord;
using Discord.Commands;
using Discord.WebSocket;
using StanDatabase.Models;

namespace StanBot.Services
{
    public class ModuleChannelService
    {
        /// <summary>
        /// Adds user access to module channel on Discord server.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="user"></param>
        /// <param name="module"></param>
        /// <returns>Returns true when it worked.</returns>
        public bool GiveUserAccessToModule(SocketCommandContext context, SocketUser user, Module module)
        {
            SocketGuildChannel socketGuildChannel = context.Guild.Channels
                .SingleOrDefault(c => c.Name.ToLower().Equals(
                    module.ChannelName.ToLower()
                )
            );
            if (socketGuildChannel != null)
            {
                socketGuildChannel.AddPermissionOverwriteAsync(user, PermissionService.GetStudentModulePermission());
                return true;
            }
            return false;
        }

        public bool RemoveUserAccessToModule(SocketCommandContext context, SocketUser user, Module module)
        {
            SocketGuildChannel socketGuildChannel = context.Guild.Channels
                .SingleOrDefault(c => c.Name.ToLower().Equals(
                    module.ChannelName.ToLower()
                )
            );
            if (socketGuildChannel != null)
            {
                socketGuildChannel.RemovePermissionOverwriteAsync(user);
                return true;
            }
            return false;
        }

        public bool DoesModuleChannelExist(SocketCommandContext context, string moduleName)
        {
            return context.Guild.Channels.FirstOrDefault(c => c.Name.ToLower().Equals(moduleName.ToLower())) != null;
        }
    }
}
