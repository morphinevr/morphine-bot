using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using MorphineBot.Services;

namespace MorphineBot.Commands
{
    public class SimpleCommands : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Help()
        {
            await Context.Channel.SendMessageAsync("**Help**\n");

            List<string> helpBuffer = new();

            foreach (var categoryGroup in Config.CommandTags)
            {
                helpBuffer.Clear();
                // helpBuffer.Add($"Category: **{categoryGroup.category}**\n```");
                helpBuffer.Add("```");

                var sortedList = categoryGroup.commands.OrderBy(item => item.Name).ToList();

                foreach (var command in sortedList)
                {
                    helpBuffer.Add($"{command.Name.PadRight(25)}{command.Description}");
                }

                // Pad the last one with backticks
                helpBuffer[^1] += "```";

                var embedBuilder = new EmbedBuilder().WithTitle($"Category • {categoryGroup.category}")
                    .WithColor(categoryGroup.color)
                    .WithDescription(String.Join('\n', helpBuffer));

                await Context.Channel.SendMessageAsync("", false, embedBuilder.Build());
            }
        }
    }
}