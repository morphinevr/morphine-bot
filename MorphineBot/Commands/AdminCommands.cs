using System;
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

        [Command("starboard_min")]
        public async Task StarboardMinStars(int threshold, [Remainder] string extra)
        {
            if (((SocketGuildUser) Context.Message.Author).GuildPermissions.Administrator)
            {
                threshold = Math.Max(threshold, 1);
                Config.ServerConfigs[Context.Guild.Id].StarboardMinimumStars = threshold;
                await Config.SaveConfig();
                await Context.Channel.SendMessageAsync($"Starboard minimum threshold set to {threshold} stars!");
            }
        }

        [Command("suicide")]
        public async Task CommitDie([Remainder] string extra)
        {
            if (Context.User.Id == Config.HYBLOCKER_ID)
            {
                await Program._client.LogoutAsync();
            }
        }

        [Command("reload")]
        public async Task ReloadCommands([Remainder] string extra)
        {
            if (((SocketGuildUser) Context.Message.Author).GuildPermissions.Administrator)
            {
                Config.LoadCommands();
            }
        }
    }
}