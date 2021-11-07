using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MorphineBot
{
    public class CommandHandler
    {
        DiscordSocketClient _client;
        public CommandService _service;
        public ServiceHandler _serviceHandler;

        public async Task InitialiseAsync(DiscordSocketClient client)
        {
            this._client = client;
            _service = new CommandService();
            _serviceHandler = new ServiceHandler(_client);
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            _client.MessageReceived += HandleCommandAsync;
            await _client.SetStatusAsync(UserStatus.Idle);
            await _client.SetGameAsync(Config.Status, null, ActivityType.Watching);
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;

            var channel = msg.Channel as SocketGuildChannel;
            var dmChannel = msg.Channel as SocketDMChannel;

            if (channel == null && dmChannel == null)
                return;

            int argPos = 0;
            if (msg.Author.IsBot)
                return;

            var context = new SocketCommandContext(_client, msg);

            if (Utils.Contains(Config.GuildWhitelist, context.Guild.Id))
            {
                Console.WriteLine(channel == null
                    ? $"(#{dmChannel.Recipient.Username}) [{msg.Author.Username}]: {msg.Content}"
                    : $"(\"{channel.Guild.Name}\" in #{channel.Name}) [{msg.Author.Username}]: {msg.Content}");

                if (msg.HasStringPrefixIgnoreCase(Config.Prefix, ref argPos) ||
                    msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
                {
                    var result = await _service.ExecuteTrimAsync(context, argPos, null);
                    if (!result.IsSuccess)
                    {
                        await Console.Error.WriteLineAsync(result.ErrorReason);
                        await _serviceHandler.HandleMessage(context);
                    }
                }
                else
                {
                    await _serviceHandler.HandleMessage(context);
                }
            }
        }
    }
}