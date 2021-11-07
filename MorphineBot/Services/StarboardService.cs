using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace MorphineBot.Services
{
    public class StarboardService : IService
    {
        private const ulong StarboardChannelId = 904448968975589447L;
        private SocketTextChannel _starboardChannel = null;
        private const string StarEmote = "⭐";
        private static readonly Color YellowColor = new Color(255, 172, 51);

        // Local temporary cache    
        private Dictionary<ulong, RestUserMessage> _starboardCache = new();

        // Shorthand to get current guild
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SocketGuild CurrentGuild(ISocketMessageChannel channel)
        {
            return ((SocketGuildChannel) channel).Guild;
        }

        public async Task ReactionAdded(IMessage message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.Emote.Name == StarEmote &&
                message.Reactions[reaction.Emote].ReactionCount >=
                Config.ServerConfigs[CurrentGuild(channel).Id].StarboardMinimumStars)
            {
                // Cache starboard channel
                FindStarboardChannel(channel);

                // Construct embed
                EmbedBuilder starboardEmbed = ConstructEmbed(message, reaction);

                // Send or edit
                await SendEmbedInChannel(message.Id, _starboardChannel, starboardEmbed);
            }
        }

        public async Task ReactionRemoved(IMessage message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.Emote.Name == StarEmote)
            {
                // Cache starboard channel
                FindStarboardChannel(channel);

                // Construct embed
                EmbedBuilder starboardEmbed = ConstructEmbed(message, reaction);

                // Send or edit
                await SendEmbedInChannel(message.Id, _starboardChannel, starboardEmbed);
            }
        }

        private void FindStarboardChannel(ISocketMessageChannel channel)
        {
            if (_starboardChannel == null)
                _starboardChannel =
                    (SocketTextChannel) CurrentGuild(channel).GetChannel(StarboardChannelId);
        }

        private EmbedBuilder ConstructEmbed(IMessage message, SocketReaction reaction)
        {
            EmbedBuilder starboardEmbed = new EmbedBuilder()
                .WithAuthor(message.Author.Username, message.Author.GetAvatarUrl())
                .WithColor(YellowColor)
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
            if (!_starboardCache.ContainsKey(id))
            {
                // Try fetching the starboard message
                RestUserMessage msg = null;
                if (Config.ServerConfigs[CurrentGuild(channel).Id].StarboardMessages.ContainsKey(id))
                {
                    msg = (RestUserMessage) await Utils.GetMessageAsync(_starboardChannel,
                        Config.ServerConfigs[CurrentGuild(channel).Id].StarboardMessages[id]);
                    await msg.ModifyAsync(msg => msg.Embed = embed.Build());
                }
                else
                {
                    msg = await _starboardChannel.SendMessageAsync("", false, embed.Build());
                    Config.ServerConfigs[CurrentGuild(channel).Id].StarboardMessages.Add(id, msg.Id);
                    await Config.SaveConfig();
                }

                _starboardCache.Add(id, msg);
            }
            else
            {
                await _starboardCache[id].ModifyAsync(msg => msg.Embed = embed.Build());
            }
        }
    }
}