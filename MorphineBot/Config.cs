using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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
        private static readonly string ConfigFolder = Path.GetFullPath(Path.Combine(AssemblyDirectory, "Resources"));
        private static readonly string ConfigFile = Path.GetFullPath(Path.Combine(ConfigFolder, "config.json"));

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

        /// <summary>
        /// Returns the executing assembly's directory
        /// </summary>
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().Location;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        static Config()
        {
            _singleton = new Config();

            if (!Directory.Exists(ConfigFolder))
                Directory.CreateDirectory(ConfigFolder);

            if (File.Exists(ConfigFile))
            {
                string json = File.ReadAllText(ConfigFile);
                _singleton = JsonConvert.DeserializeObject<Config>(json);
            }
            else
            {
                Console.WriteLine($"[WARN]: Missing config.json!");

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
            await File.WriteAllTextAsync(ConfigFile, json);
        }
    }
}