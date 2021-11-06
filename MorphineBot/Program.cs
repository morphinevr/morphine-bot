using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace MorphineBot
{
    class Program
    {
        public static DiscordSocketClient _client;
        public static CommandHandler _handler;

        private const string DmLeaveMessage = "no.";
        private const string AuditLogLeaveReason = "This is a private bot.";

        static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Morphine Bot";
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            _client.Log += Log;
            _client.JoinedGuild += OnJoinGuild;
            _client.LeftGuild += OnLeaveGuild;
            _client.Connected += CleanToWhitelist;

            await _client.LoginAsync(TokenType.Bot, Config.Token);
            await _client.StartAsync();
            _handler = new CommandHandler();
            await _handler.InitialiseAsync(_client);
            await Task.Delay(-1);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine($"[{msg.Source}] [{msg.Severity}]: {msg.Message}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Analyse the guilds we're in and leave any guilds which aren't whitelisted
        /// </summary>
        private async Task CleanToWhitelist()
        {
            foreach (SocketGuild theGuild in _client.Guilds)
            {
                if (Utils.Contains(Config.GuildWhitelist, theGuild.Id))
                {
                    //Also init servers lol
                    if (!Config.ServerConfigs.ContainsKey(theGuild.Id))
                    {
                        Config.ServerConfigs.Add(theGuild.Id, new ServerConfig());
                        await Config.SaveConfig();
                    }
                }
                else
                {
                    if (theGuild.Owner == null && theGuild.DefaultChannel != null)
                        await theGuild.DefaultChannel.SendMessageAsync(DmLeaveMessage);
                    else
                        await theGuild.Owner.SendMessageAsync(DmLeaveMessage);

                    await theGuild.LeaveAsync(new RequestOptions() {AuditLogReason = AuditLogLeaveReason});
                }
            }
        }

        /// <summary>
        /// Clear the config if the Bot leaves a guild.
        /// </summary>
        private async Task OnLeaveGuild(SocketGuild theGuild)
        {
            if (Config.ServerConfigs.ContainsKey(theGuild.Id))
            {
                Config.ServerConfigs.Remove(theGuild.Id);
                await Config.SaveConfig();
            }
        }

        /// <summary>
        /// Detect if someone generated a Bot token and tried to invite the bot
        /// If we notice that someone tried taking a copy of the bot, then leave the server and attempt dming the owner
        /// </summary>
        private async Task OnJoinGuild(SocketGuild theGuild)
        {
            if (Utils.Contains(Config.GuildWhitelist, theGuild.Id))
            {
                if (!Config.ServerConfigs.ContainsKey(theGuild.Id))
                {
                    Config.ServerConfigs.Add(theGuild.Id, new ServerConfig());
                    await Config.SaveConfig();
                }
            }
            else
            {
                await theGuild.Owner.SendMessageAsync(DmLeaveMessage);
                await theGuild.LeaveAsync(new RequestOptions() {AuditLogReason = AuditLogLeaveReason});
            }
        }
    }
}