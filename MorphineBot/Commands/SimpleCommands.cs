using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using MorphineBot.Services;

namespace MorphineBot.Commands
{
    public class SimpleCommands : ModuleBase<SocketCommandContext>
    {
        // Reflection stuff
        List<CommandInfo> commands = Program._handler._service.Commands.ToList();

        List<CommandInfo> adminCommands = new List<CommandInfo>();
        List<CommandInfo> ownerCommands = new List<CommandInfo>();
        List<CommandInfo> miscCommands = new List<CommandInfo>();

        private void InitCommandsLists()
        {
            //Separate the commands by group
            for (int i = 0; i < commands.Count; i++)
            {
                bool skip = false;
                var parameters = commands[i].Attributes;
                for (int j = 0; j < parameters.Count; j++)
                {
                    if (parameters[j].GetType() == typeof(OwnerCommand))
                    {
                        ownerCommands.Add(commands[i]);
                        skip = true;
                    }
                    else if (parameters[j].GetType() == typeof(AdminCommand))
                    {
                        adminCommands.Add(commands[i]);
                        skip = true;
                    }
                }

                if (!skip)
                    miscCommands.Add(commands[i]);
            }
        }


        [Command("help")]
        [Summary("Ouputs this message!")]
        public async Task Help()
        {
            if (ownerCommands.Count == 0)
                InitCommandsLists();

            await Context.Channel.SendMessageAsync("**Help**\n");

            List<string> helpBuffer = new();

            foreach (var categoryGroup in Config.CommandTags)
            {
                helpBuffer.Clear();
                // helpBuffer.Add($"Category: **{categoryGroup.category}**\n```");
                helpBuffer.Add("```");

                var commandListBuffer = categoryGroup.commands;

                // Patch the groups up
                switch (categoryGroup.category)
                {
                    case "misc":
                        foreach (var command in miscCommands)
                        {
                            commandListBuffer.Add(new CommandTag(command.Name, command.Summary,
                                command.Aliases.ToArray()));
                        }

                        break;
                    case "admin":
                        foreach (var command in adminCommands)
                        {
                            commandListBuffer.Add(new CommandTag(command.Name, command.Summary,
                                command.Aliases.ToArray()));
                        }

                        if (Context.User.Id == Config.HYBLOCKER_ID)
                        {
                            foreach (var command in ownerCommands)
                            {
                                commandListBuffer.Add(new CommandTag(command.Name, command.Summary,
                                    command.Aliases.ToArray()));
                            }
                        }

                        break;
                }

                var sortedList = commandListBuffer.OrderBy(item => item.Name).ToList();

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