﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private static readonly Color YELLOW_COLOR = new Color(255, 172, 51);
        private static readonly CultureInfo EN_US_GLOB = CultureInfo.CreateSpecificCulture("en-US");

        // Local temporary cache    
        private Dictionary<ulong, RestUserMessage> starboard_cache = new();

        // Shorthand to get current guild
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SocketGuild CurrentGuild(ISocketMessageChannel channel)
        {
            return ((SocketGuildChannel) channel).Guild;
        }

        public async Task ReactionAdded(IMessage message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.Emote.Name == STAR_EMOTE &&
                message.Reactions[reaction.Emote].ReactionCount >=
                Config.ServerConfigs[CurrentGuild(channel).Id].StarboardMinimumStars)
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
                    (SocketTextChannel) CurrentGuild(channel).GetChannel(STARBOARD_CHANNEL_ID);
        }

        private EmbedBuilder ConstructEmbed(IMessage message, SocketReaction reaction)
        {
            EmbedBuilder starboardEmbed = new EmbedBuilder()
                .WithAuthor(message.Author.Username, message.Author.GetAvatarUrl())
                .WithColor(YELLOW_COLOR)
                .WithDescription($"{message.Content}\n\n[Link]({message.GetJumpUrl()})")
                .WithTimestamp(message.Timestamp)
                .WithFooter($"#{message.Channel.Name}");

            // Add the reactions field
            starboardEmbed.AddField("Reactions", message.Reactions[reaction.Emote].ReactionCount);

            // Add attachments if the message has any
            if (message.Attachments.Count > 0)
                starboardEmbed = starboardEmbed.WithImageUrl(message.Attachments.First().Url);

            return starboardEmbed;
        }

        private async Task SendEmbedInChannel(ulong id, ISocketMessageChannel channel, EmbedBuilder embed)
        {
            if (!starboard_cache.ContainsKey(id))
            {
                // Try fetching the starboard message
                RestUserMessage msg = null;
                if (Config.ServerConfigs[CurrentGuild(channel).Id].StarboardMessages.ContainsKey(id))
                {
                    msg = (RestUserMessage) await Utils.GetMessageAsync(STARBOARD_CHANNEL,
                        Config.ServerConfigs[CurrentGuild(channel).Id].StarboardMessages[id]);
                    await msg.ModifyAsync(msg => msg.Embed = embed.Build());
                }
                else
                {
                    msg = await STARBOARD_CHANNEL.SendMessageAsync("", false, embed.Build());
                    Config.ServerConfigs[CurrentGuild(channel).Id].StarboardMessages.Add(id, msg.Id);
                    await Config.SaveConfig();
                }

                starboard_cache.Add(id, msg);
            }
            else
            {
                await starboard_cache[id].ModifyAsync(msg => msg.Embed = embed.Build());
            }
        }
    }
}