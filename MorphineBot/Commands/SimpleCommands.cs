using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace MorphineBot.Commands
{
    public class SimpleCommands : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Help()
        {
            await Context.Channel.SendMessageAsync("```\n//TODO: help auto-gen or something```");
        }
    }
}