using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MorphineBot.Commands
{
    public class AdminCommands : ModuleBase<SocketCommandContext>
    {
        [Command("status")]
        public async Task ChangeStatus([Remainder] string newStatus)
        {
            if (((SocketGuildUser) Context.Message.Author).GuildPermissions.Administrator)
            {
                Config.Status = newStatus;
                await Config.SaveConfig();
                await Program._client.SetGameAsync(Config.Status, null, ActivityType.Watching);
                await Context.Channel.SendMessageAsync($"Set status to ```{newStatus}```");
            }
        }
    }
}