using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace MorphineBot.Services
{
    /// <summary>
    /// Commands in this context are essentially tags, as theyre a copy paste message with a known shorthand, shorthand being the command
    /// </summary>
    public class CommandTagService : IService
    {
        public async Task ProcessMessage(SocketCommandContext context)
        {
            // Check if this message is a command or a normal regular message
            int argPos = 0;
            if (context.Message.HasStringPrefixIgnoreCase(Config.Prefix, ref argPos) ||
                context.Message.HasMentionPrefix(Program._client.CurrentUser, ref argPos))
            {
                // Generate variables to make life easier
                var parameters = context.Message.Content.Substring(argPos).Trim().Split(' ');
                var command = parameters[0].ToLower();
                parameters = parameters.Skip(1).ToArray();

                // TODO: complex commands
                for (int i = 0; i < Config.CommandTags.Count; i++)
                {
                    for (int j = 0; j < Config.CommandTags[i].commands.Count; j++)
                    {
                        
                    if (Config.CommandTags[i].commands[j].Name.ToLower() == command ||
                        (Config.CommandTags[i].commands[j].Alias != null && Config.CommandTags[i].commands[j].Alias.Contains(command)))
                    {
                        await context.Channel.SendMessageAsync(Config.CommandTags[i].commands[j].Content.Content ?? string.Empty);

                        // exit; we don't need to loop through the rest of the commands
                        return;
                    }
                    }
                }
            }
        }
    }
    
    public class CommandTagCategory
    {
        public string category { get; set; }
        public uint color { get; set; }
        public List<CommandTag> commands { get; set; }
    }

    public struct CommandTag
    {
        public string Name;
        public string[] Alias;

        public string Description;

        // TODO: category
        public MessageObject Content;

        public CommandTag(string name = null, string description = null, string[] aliases = null)
        {
            this.Name = name;
            Description = description;
            Alias = aliases ?? new string[] { };
            Content = new MessageObject();
        }

        public CommandTag(string name, MessageObject content) : this()
        {
            Name = name;
            Content = content;
        }
    }

    public struct MessageObject
    {
        // TODO: more than just text
        public string Content;

        public MessageObject(string content = null)
        {
            Content = content;
        }
    }
}