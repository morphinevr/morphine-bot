using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

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
        
        // Execute a command, but trim the string because AAAAAAA
        public static Task<IResult> ExecuteTrimAsync(
            this CommandService commandService,
            ICommandContext context,
            int argPos,
            IServiceProvider services,
            MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception)
        {
            return commandService.ExecuteAsync(context, context.Message.Content.Substring(argPos).Trim(), services, multiMatchHandling);
        }
    }
}