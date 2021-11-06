using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace MorphineBot.Services
{
    public class StarboardService : IService
    {
        public async Task ProcessMessage(SocketCommandContext context)
        {
            Console.WriteLine("[Starboard Service] " + context.Message);
        }
    }
}