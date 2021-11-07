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
            // var embedBuilder = new EmbedBuilder().WithTitle("Help").WithDescription("i dunno 💀");
            
            // await Context.Channel.SendMessageAsync("", false, embedBuilder.Build());
        }
    }
}