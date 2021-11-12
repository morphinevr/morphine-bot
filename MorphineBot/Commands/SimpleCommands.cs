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

        private static readonly uint[] PrideFlagColor = new[]
        {
            16711692u,
            16738816u,
            16768256u,
            50711u,
            23039u,
            12255487u,
        };

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
        [Summary("Outputs this message!")]
        public async Task Help()
        {
            if (ownerCommands.Count == 0)
                InitCommandsLists();

            await Context.Channel.SendMessageAsync("**Help**\n");

            List<string> helpBuffer = new();
            bool isPrideMonth = DateTime.UtcNow.Month == 6;

            for (var i = 0; i < Config.CommandTags.Count; i++)
            {
                var categoryGroup = Config.CommandTags[i];
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

                // pride month <3
                if (isPrideMonth && i < 6)
                    categoryGroup.color = PrideFlagColor[i];

                // sort commands alphabetically
                var sortedList = commandListBuffer.OrderBy(item => item.Name).ToList();

                // add commands to embed
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