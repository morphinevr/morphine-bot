using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MorphineBot.Services;
using Newtonsoft.Json;

namespace MorphineBot
{
    // Individual server config
    public class ServerConfig
    {
        /// <summary>
        /// Starboard minimum stars
        /// </summary>
        public int StarboardMinimumStars = 3;

        /// <summary>
        /// A list of role assignments
        /// </summary>
        public Dictionary<ulong, RoleAssignment> RoleAssignments = new();

        /// <summary>
        /// A list of message id pairs, with key being the message being starred and the value associated with it being the starboard message ID
        /// </summary>
        public Dictionary<ulong, ulong> StarboardMessages = new();
    }

    // Individual role properties
    public class RoleAssignment
    {
        public ulong channel = 0;
        public List<(ulong RoleID, string Emote)> EmoteRoleIdPair = new();
    }

    public class Config
    {
        // Singleton
        private static Config _singleton = new Config();

        // Actual variables, only visible to singleton, but get proxied through static vars
        [JsonProperty] private string _token = "BRUH";
        [JsonProperty] private string _prefix = "pissy ";
        [JsonProperty] private string _status = "drugs4vr 😎";

        [JsonProperty] private ulong[] _guildWhitelist =
        {
            902897386232348772L, // Morphine server
        };

        [JsonProperty] private Dictionary<ulong, ServerConfig> _serverConfig = new();
        public static List<CommandTagCategory> CommandTags = new();

        // Helper vars for config related files
        private const string ConfigFolder = "Resources";
        private const string ConfigFile = "config.json";
        private const string CommandsFile = "commands.json";
        private static readonly string ConfigFileFullPath = Path.Combine(ConfigFolder, ConfigFile);
        private static readonly string CommandsFileFullPath = Path.Combine(ConfigFolder, CommandsFile);

        public const ulong HYBLOCKER_ID = 346338830011596800L;

        public static string Token => _singleton._token;

        public static string Status
        {
            get { return _singleton._status; }
            set { _singleton._status = value; }
        }

        public static string Prefix
        {
            get { return _singleton._prefix; }
            set { _singleton._prefix = value; }
        }

        public static ulong[] GuildWhitelist => _singleton._guildWhitelist;

        public static Dictionary<ulong, ServerConfig> ServerConfigs
        {
            get { return _singleton._serverConfig; }
            set { _singleton._serverConfig = value; }
        }

        static Config()
        {
            _singleton = new Config();

            // Ensure the config directory exists
            if (!Directory.Exists(ConfigFolder))
            {
                Directory.CreateDirectory(ConfigFolder);
                Console.WriteLine($"[WARN]: Directory {ConfigFolder} was not found!!");
            }

            // Try loading the config
            if (File.Exists(ConfigFileFullPath))
            {
                string json = File.ReadAllText(ConfigFileFullPath);
                _singleton = JsonConvert.DeserializeObject<Config>(json);
            }
            else
            {
                Console.WriteLine($"[WARN]: Missing {ConfigFile}!");

                // Save dummy config
                _singleton = new Config();
                SaveConfig().GetAwaiter().GetResult();
            }

            LoadCommands();
        }

        public static async Task SaveConfig()
        {
            if (!Directory.Exists(ConfigFolder))
                Directory.CreateDirectory(ConfigFolder);
            string json = JsonConvert.SerializeObject(_singleton, Formatting.Indented);
            await File.WriteAllTextAsync(ConfigFileFullPath, json);
        }

        public static void LoadCommands()
        {
            // Try loading the commands list
            if (File.Exists(CommandsFileFullPath))
            {
                string json = File.ReadAllText(CommandsFileFullPath);
                CommandTags = JsonConvert.DeserializeObject<List<CommandTagCategory>>(json);
            }
            else
                Console.WriteLine($"[WARN]: Missing {CommandsFile}!");
        }
    }
}