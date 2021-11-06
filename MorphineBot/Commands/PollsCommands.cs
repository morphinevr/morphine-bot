using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace MorphineBot.Commands
{
    public class PollsCommands : ModuleBase<SocketCommandContext>
    {
        [Command("polls")]
        public async Task GenPollReactions(string count = "2")
        {
            if (Context.User.Id == Context.Guild.OwnerId)
            {
                var rectionRoot =
                    await Context.Message.Channel.SendMessageAsync(
                        "React with the corresponding emoji to pick your vote!");

                int numReactions = Math.Clamp(int.Parse(count), 0, 10);
                await Context.Message.DeleteAsync();

                StringBuilder emojiUnicode = new StringBuilder("\u0030\u20E3");

                for (int i = 0; i < numReactions; i++)
                {
                    emojiUnicode[0] = (char) ('\u0030' + i + 1);
                    await rectionRoot.AddReactionAsync(new Emoji(emojiUnicode.ToString()));
                }
            }
        }
    }
}