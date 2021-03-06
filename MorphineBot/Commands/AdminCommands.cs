using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MorphineBot.Commands
{
    public class AdminCommands : ModuleBase<SocketCommandContext>
    {

        [Command("echo")]
        [Summary("Echoes the message given")]
        [AdminCommand]
        public async Task EchoMessage([Remainder] string extra = "")
        {
            if (Context.User.Id == Config.HYBLOCKER_ID)
            {
                await Context.Channel.SendMessageAsync(extra);
                await Context.Message.DeleteAsync();
            }
        }
        
        [Command("status")]
        [Summary("Changes the bot's status")]
        [OwnerCommand]
        public async Task ChangeStatus([Remainder] string newStatus)
        {
            if (Context.User.Id == Config.HYBLOCKER_ID)
            {
                Config.Status = newStatus;
                await Config.SaveConfig();
                await Program._client.SetGameAsync(Config.Status, null, ActivityType.Watching);
                await Context.Channel.SendMessageAsync($"Set status to ```{newStatus}```");
            }
        }

        [Command("starboard_min")]
        [Summary("Sets the minimum starboard count")]
        [AdminCommand]
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
        [Summary("Kills the bot lmao")]
        [OwnerCommand]
        public async Task CommitDie([Remainder] string extra)
        {
            if (Context.User.Id == Config.HYBLOCKER_ID)
            {
                await Program._client.LogoutAsync();
                
                // Destructive trolling :D
                Process.GetCurrentProcess().Kill();
            }
        }

        [Command("reload")]
        [Summary("Reloads tags")]
        [AdminCommand]
        public async Task ReloadCommands([Remainder] string extra = "")
        {
            if (Context.User.Id == Config.HYBLOCKER_ID)
            {
                Config.LoadCommands();

                await Context.Channel.SendMessageAsync("Successfully reloaded all tags!");
            }
        }
    }
}