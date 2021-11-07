using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace MorphineBot.Services
{
    public class RolemenuService : IService
    {
        // Shorthand to get current guild
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SocketGuild CurrentGuild(ISocketMessageChannel channel)
        {
            return ((SocketGuildChannel) channel).Guild;
        }

        public async Task ReactionAdded(IMessage message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            SocketGuild currentGuild = CurrentGuild(channel);
            if (Config.ServerConfigs[currentGuild.Id].RoleAssignments.ContainsKey(message.Id))
            {
                var channelReactionRoles =
                    Config.ServerConfigs[currentGuild.Id].RoleAssignments[message.Id].EmoteRoleIdPair;

                for (int i = 0; i < channelReactionRoles.Count; i++)
                {
                    if (channelReactionRoles[i].Emote == reaction.Emote.ToString())
                    {
                        await ((SocketGuildUser) reaction.User).AddRoleAsync(
                            currentGuild.GetRole(channelReactionRoles[i].RoleID));

                        // Exit early, as these events only contain a single emote
                        return;
                    }
                }
            }
        }

        public async Task ReactionRemoved(IMessage message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            SocketGuild currentGuild = CurrentGuild(channel);
            if (Config.ServerConfigs[currentGuild.Id].RoleAssignments.ContainsKey(message.Id))
            {
                var channelReactionRoles =
                    Config.ServerConfigs[currentGuild.Id].RoleAssignments[message.Id].EmoteRoleIdPair;

                for (int i = 0; i < channelReactionRoles.Count; i++)
                {
                    if (channelReactionRoles[i].Emote == reaction.Emote.ToString())
                    {
                        await ((SocketGuildUser) reaction.User).RemoveRoleAsync(
                            currentGuild.GetRole(channelReactionRoles[i].RoleID));

                        // Exit early, as these events only contain a single emote
                        return;
                    }
                }
            }
        }
    }
}