using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace MorphineBot.Commands
{
    public class SimpleCommands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            await Context.Channel.SendMessageAsync("pong");
        }
    }
}