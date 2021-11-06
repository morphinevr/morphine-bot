using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;

namespace MorphineBot
{
    public class ServiceHandler
    {
        private static List<IService> _services = new();

        /// <summary>
        /// Finds all services
        /// This syntax is used so that we don't have to call some connectServices function, rather accessing the class will automatically call this,
        /// leading to a cleaner API.
        /// </summary>
        static ServiceHandler()
        {
            // Get all classes implementing IService, and store them in _services

            _services = (from t in Assembly.GetExecutingAssembly().GetTypes()
                where t.GetInterfaces().Contains(typeof(IService))
                      && t.GetConstructor(Type.EmptyTypes) != null
                select Activator.CreateInstance(t) as IService).ToList();
        }

        public async Task HandleMessage(SocketCommandContext context)
        {
            for (int i = 0; i < _services.Count; i++)
            {
                await _services[i].ProcessMessage(context);
            }
        }
    }

    public interface IService
    {
        Task ProcessMessage(SocketCommandContext context);
    }
}