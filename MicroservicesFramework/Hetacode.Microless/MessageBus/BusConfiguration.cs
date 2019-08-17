using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Hetacode.Microless.Abstractions.Filters;
using Hetacode.Microless.Abstractions.Managers;
using Hetacode.Microless.Abstractions.MessageBus;
using Hetacode.Microless.Extensions;
using Hetacode.Microless.Managers;
using Newtonsoft.Json;

namespace Hetacode.Microless.MessageBus
{
    public class BusConfiguration : IBusConfiguration, IBusSubscriptions
    {
        private IQueueProvider provider;
        private IFunctionsManager _functionsManager;

        public BusConfiguration(IFiltersManager filtersManager, IFunctionsManager functionsManager)
            => (Filters, _functionsManager) = (filtersManager, functionsManager);

        public IQueueProvider Provider
        {
            get => provider;
            set
            {
                provider = value;
                provider.Init();
            }
        }

        public IFiltersManager Filters { get; }

        public void AddReceiver(string name, Func<string, object, Dictionary<string, string>, Task> messageCallback)
        {
            Provider.AddReceiver(name, async (json, headers) =>
            {
                //TODO: move to some helper
                var rawMessage = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                var type = rawMessage["_type"];
                var message = JsonConvert.DeserializeObject(json, Type.GetType(type));

                message = await Filters.ProcessIncoming(message);

                await messageCallback(name, message, headers);
            });
        }

        public void Send(string name, object message, Dictionary<string, string> headers = null)
        {
            //TODO: move to some helper
            var type = message.GetType().AssemblyQualifiedName;
            var dynamicMessage = message.ToExpandoObject();
            dynamicMessage.TryAdd("_type", type);
            var json = JsonConvert.SerializeObject(dynamicMessage);

            Provider.Send(name, json, headers);
        }
    }
}
