using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NLog;

namespace StanBot.Services
{
    public class RoleService
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Adds role when user doesn't have it.
        /// Does nothing when user already has it.
        /// </summary>
        /// <param name="socketGuildUser"></param>
        /// <param name="role"></param>
        public void AddRole(SocketGuildUser socketGuildUser, IRole role)
        {
            if (!socketGuildUser.Roles.Contains(role))
            {
                socketGuildUser.AddRoleAsync(role);
                _logger.Debug($"Added role \"{role}\" to user \"{socketGuildUser.Username}\"");
            }
        }

        public void AddRole(ICommandContext context, SocketGuildUser socketGuildUser, string roleName)
        {
            AddRole(socketGuildUser, GetRoleByName(context, roleName));
        }

        /// <summary>
        /// Removes role when user has it.
        /// Does nothing when user doesn't have it.
        /// </summary>
        /// <param name="socketGuildUser"></param>
        /// <param name="role"></param>
        public void RemoveRole(SocketGuildUser socketGuildUser, IRole role)
        {

            if (!socketGuildUser.Roles.Contains(role))
            {
                socketGuildUser.RemoveRoleAsync(role);
                _logger.Debug($"Removed role \"{role}\" to user \"{socketGuildUser.Username}\"");
            }
        }

        public void RemoveRole(ICommandContext context, SocketGuildUser socketGuildUser, string roleName)
        {
            RemoveRole(socketGuildUser, GetRoleByName(context, roleName));
        }

        public IRole GetRoleByName(ICommandContext context, string roleName)
        {
            return context.Guild.Roles.Single(r => r.Name.Equals(roleName));
        }
    }
}
