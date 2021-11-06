using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

namespace MorphineBot.Services
{
    public class StarboardService : IService
    {
        private const ulong STARBOARD_CHANNEL_ID = 904448968975589447L;
        private SocketTextChannel STARBOARD_CHANNEL = null;
        private const string STAR_EMOTE = "⭐";
        private readonly static Color YELLOW_COLOR = new Color(255, 172, 51);
        private readonly static CultureInfo EN_US_GLOB = CultureInfo.CreateSpecificCulture("en-US");

        private Dictionary<ulong, RestUserMessage> starboard_cache = new();

        public async Task ProcessMessage(SocketCommandContext context)
        {
            Console.WriteLine("[Starboard Service] " + context.Message);
        }

        public async Task ReactionAdded(IMessage message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.Emote.Name == STAR_EMOTE &&
                message.Reactions[reaction.Emote].ReactionCount >=
                Config.ServerConfigs[((SocketGuildChannel) channel).Guild.Id].StarboardMinimumStars)
            {
                // Cache starboard channel
                FindStarboardChannel(channel);

                // Construct embed
                EmbedBuilder starboardEmbed = ConstructEmbed(message, reaction);
                ;

                // Send or edit
                await SendEmbedInChannel(message.Id, STARBOARD_CHANNEL, starboardEmbed);
            }
        }

        public async Task ReactionRemoved(IMessage message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.Emote.Name == STAR_EMOTE)
            {
                // Cache starboard channel
                FindStarboardChannel(channel);

                // Construct embed
                EmbedBuilder starboardEmbed = ConstructEmbed(message, reaction);

                // Send or edit
                await SendEmbedInChannel(message.Id, STARBOARD_CHANNEL, starboardEmbed);
            }
        }

        private void FindStarboardChannel(ISocketMessageChannel channel)
        {
            if (STARBOARD_CHANNEL == null)
                STARBOARD_CHANNEL =
                    (SocketTextChannel) ((SocketGuildChannel) channel).Guild.GetChannel(STARBOARD_CHANNEL_ID);
        }

        private EmbedBuilder ConstructEmbed(IMessage message, SocketReaction reaction)
        {
            EmbedBuilder starboardEmbed = new EmbedBuilder()
                .WithAuthor(message.Author.Username, message.Author.GetAvatarUrl()).WithColor(YELLOW_COLOR)
                .WithDescription(message.Content)
                .WithFooter($"Sent on {message.Timestamp.ToUniversalTime().ToString("g", EN_US_GLOB)}");

            starboardEmbed.AddField("Reactions", message.Reactions[reaction.Emote].ReactionCount);

            if (message.Attachments.Count > 0)
                starboardEmbed = starboardEmbed.WithImageUrl(message.Attachments.First().Url);

            return starboardEmbed;
        }

        private async Task SendEmbedInChannel(ulong id, ISocketMessageChannel channel, EmbedBuilder embed)
        {
            if (!starboard_cache.ContainsKey(id))
                starboard_cache.Add(id,
                    await STARBOARD_CHANNEL.SendMessageAsync("", false, embed.Build()));
            else
                await starboard_cache[id].ModifyAsync(msg => msg.Embed = embed.Build());
        }
    }
}