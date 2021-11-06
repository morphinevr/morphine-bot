using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MorphineBot.Commands
{
    public class AdminCommands : ModuleBase<SocketCommandContext>
    {
        private static bool IsAdmin(SocketGuildUser user)
        {
            return user.GuildPermissions.Administrator;
        }

        [Command("status")]
        public async Task ChangeStatus([Remainder] string newStatus)
        {
            if (IsAdmin((SocketGuildUser) Context.User))
            {
                Config.Status = newStatus;
                await Config.SaveConfig();
                await Program._client.SetGameAsync(Config.Status, null, ActivityType.Watching);
            }
        }
    }
}