using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Commands;

namespace MorphineBot.Commands
{
    public class NeofetchCommand : ModuleBase<SocketCommandContext>
    {
        [Command("neofetch")]
        [Summary("for linux nerds")]
        public async Task Neofetch([Remainder] string args = "")
        {
            using (Process p = new Process())
            {
                p.StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "neofetch",
                    Arguments = "--stdout" + (args.Length == 0 ? "" : args),
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };

                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                output = Regex.Replace(output, @"\[[0-9]+m", "").Trim();
                p.WaitForExit();

                await Context.Channel.SendMessageAsync($"```\n{output}\n```");
            }
        }
    }
}