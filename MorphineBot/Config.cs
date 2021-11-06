using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MorphineBot
{
    // Individual server config
    public class ServerConfig
    {
        public Dictionary<ulong, RoleAssignment> RoleAssignments = new();
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
        [JsonProperty] private string _prefix = "pissy";
        [JsonProperty] private string _status = "drugs4vr 😎";

        [JsonProperty] private ulong[] _guildWhitelist =
        {
            902897386232348772L, // Morphine server
        };

        [JsonProperty] private Dictionary<ulong, ServerConfig> _serverConfig = new();

        // Helper vars for config related files
        private const string ConfigFolder = "Resources";
        private const string ConfigFile = "config.json";
        private static readonly string ConfigFileFullPath = Path.Combine(ConfigFolder, ConfigFile);

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

            if (!Directory.Exists(ConfigFolder))
                Directory.CreateDirectory(ConfigFolder);

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
        }

        public static async Task SaveConfig()
        {
            if (!Directory.Exists(ConfigFolder))
                Directory.CreateDirectory(ConfigFolder);
            string json = JsonConvert.SerializeObject(_singleton, Formatting.Indented);
            await File.WriteAllTextAsync(ConfigFileFullPath, json);
        }
    }
}