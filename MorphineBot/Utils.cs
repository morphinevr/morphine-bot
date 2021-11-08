using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MorphineBot
{
    public static class Utils
    {
        public static bool Contains<T>(T[] arr, T val)
        {
            return Array.Exists(arr, element => element.Equals(val));
        }

        // Check if a command starts with a prefix, case-insensitive
        public static bool HasStringPrefixIgnoreCase(
            this IUserMessage msg,
            string str,
            ref int argPos,
            StringComparison comparisonType = StringComparison.Ordinal)
        {
            string content = msg.Content;
            if (string.IsNullOrEmpty(content) || !content.ToLower().StartsWith(str.ToLower(), comparisonType))
                return false;
            argPos = str.Length;
            return true;
        }

        public static async Task<IMessage> GetMessageAsync(ISocketMessageChannel channel, ulong messageId)
        {
            // cache the message
            IMessage msg = channel.GetCachedMessage(messageId);
            if (msg == null)
                msg = await channel.GetMessageAsync(messageId);

            return msg;
        }
    }
}