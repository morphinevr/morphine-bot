using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MorphineBot.Commands
{
    public class RolemenuCommands : ModuleBase<SocketCommandContext>
    {
        private static readonly Regex RegexEmoteIdFetch = new Regex("[^:]+[0-9]+");

        [Command("roles")]
        [Alias("rolemenu", "role")]
        [Summary("format: EMOTE @role EMOTE @role ...")]
        public async Task GenPollReactions([Remainder] string msg)
        {
            // Perms
            if (!((SocketGuildUser) Context.Message.Author).GuildPermissions.ManageRoles)
                return;

            var assignments = new RoleAssignment();
            var messageComponents = Regex.Replace(msg, @"\s+", " ").Trim().Split(" ");
            var roleDescriptionBuilder = new StringBuilder();

            // Parse message
            for (int i = 0; i < messageComponents.Length; i += 2)
            {
                var emoji = messageComponents[i];
                var roleId =
                    ulong.Parse(messageComponents[i + 1].Replace(">", string.Empty).Replace("<@&", string.Empty));

                // List role emoji pair
                assignments.EmoteRoleIdPair.Add((roleId, emoji));

                // Append string
                if (i != 0)
                    roleDescriptionBuilder.Append("\n");
                roleDescriptionBuilder.Append(emoji); // Emoji
                roleDescriptionBuilder.Append("    "); // Padding
                roleDescriptionBuilder.Append(Context.Guild.GetRole(roleId).Name); // Role
            }

            // Create embed
            var embed = new EmbedBuilder();
            embed.Author = new EmbedAuthorBuilder().WithName(Context.User.Username)
                .WithIconUrl(Context.User.GetAvatarUrl());
            embed.Title = "Role Menu";
            embed.WithColor(new Color(52, 164, 255));
            embed.WithDescription(roleDescriptionBuilder.ToString());

            var rolesMessage = await Context.Channel.SendMessageAsync(null, false, embed.Build());

            // Add reactions
            for (int i = 0; i < assignments.EmoteRoleIdPair.Count; i++)
            {
                // If guild emote
                if (assignments.EmoteRoleIdPair[i].Emote.StartsWith('<') &&
                    assignments.EmoteRoleIdPair[i].Emote.Contains(':'))
                {
                    ulong id = ulong.Parse(RegexEmoteIdFetch.Match(assignments.EmoteRoleIdPair[i].Emote).ToString());
                    var guildEmote = await Context.Guild.GetEmoteAsync(id);
                    await rolesMessage.AddReactionAsync(guildEmote);
                }
                // If normal emoji (eg :skull: or 😎 )
                else
                    await rolesMessage.AddReactionAsync(new Emoji(assignments.EmoteRoleIdPair[i].Emote));
            }

            // Store by channel id
            assignments.channel = Context.Channel.Id;

            // Remember role pairs
            Config.ServerConfigs[Context.Guild.Id].RoleAssignments.Add(rolesMessage.Id, assignments);
            await Config.SaveConfig();

            // Remove original message
            await Context.Message.DeleteAsync();
        }
    }
}