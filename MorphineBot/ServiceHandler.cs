using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MorphineBot
{
    public class ServiceHandler
    {
        private List<IService> _services = new();
        private DiscordSocketClient _client;

        /// <summary>
        /// Finds all services
        /// This syntax is used so that we don't have to call some connectServices function, rather accessing the class will automatically call this,
        /// leading to a cleaner API.
        /// </summary>
        public ServiceHandler(DiscordSocketClient client)
        {
            this._client = client;
            
            // Get all classes implementing IService, and store them in _services

            _services = (from t in Assembly.GetExecutingAssembly().GetTypes()
                where t.GetInterfaces().Contains(typeof(IService))
                      && t.GetConstructor(Type.EmptyTypes) != null
                select Activator.CreateInstance(t) as IService).ToList();
            
            client.ReactionAdded += HandleReactionAdded;
            client.ReactionRemoved += HandleReactionRemoved;
        }

        public async Task HandleMessage(SocketCommandContext context)
        {
            for (int i = 0; i < _services.Count; i++)
            {
                await _services[i].ProcessMessage(context);
            }
        }

        private async Task HandleReactionAdded(Cacheable<IUserMessage, ulong> cached, ISocketMessageChannel channel, SocketReaction reaction)
        {
            // cache the message
            IMessage msg = await Utils.GetMessageAsync(channel, reaction.MessageId);
            
            for (int i = 0; i < _services.Count; i++)
            {
                await _services[i].ReactionAdded(msg, channel, reaction);
            }
        }

        private async Task HandleReactionRemoved(Cacheable<IUserMessage, ulong> cached, ISocketMessageChannel channel, SocketReaction reaction)
        {
            // cache the message
            IMessage msg = await Utils.GetMessageAsync(channel, reaction.MessageId);
            
            for (int i = 0; i < _services.Count; i++)
            {
                await _services[i].ReactionRemoved(msg, channel, reaction);
            }
        }
    }

    public interface IService
    {
        public Task ProcessMessage(SocketCommandContext context)
        {
            return Task.CompletedTask;
        }

        public Task ReactionAdded(IMessage message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            return Task.CompletedTask;
        }
        
        public Task ReactionRemoved(IMessage message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            return Task.CompletedTask;
        }
    }
}