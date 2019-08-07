using System;
using System.Collections.Generic;
using System.Reflection;
using Hetacode.Microless.Abstractions.Filters;
using Hetacode.Microless.Abstractions.MessageBus;
using Hetacode.Microless.Extensions;
using Newtonsoft.Json;

namespace Hetacode.Microless.MessageBus
{
    public class BusConfiguration : IBusConfiguration, IBusSubscriptions
    {
        private IQueueProvider provider;

        public BusConfiguration(IFiltersManager filtersManager)
            => Filters = filtersManager;

        public IQueueProvider Provider
        {
            get => provider; set
            {
                provider = value;
                provider.Init();
            }
        }

        public IFiltersManager Filters { get; }

        public void AddReceiver(string name, Action<object> messageCallback)
        {
            Provider.AddReceiver(name, json =>
            {
                //TODO: move to some helper
                var rawMessage = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                var type = rawMessage["_type"];
                var message = JsonConvert.DeserializeObject(json, Type.GetType(type));
                messageCallback(message);
            });
        }

        public void Send(string name, object message)
        {
            //TODO: move to some helper
            var type = message.GetType().AssemblyQualifiedName;
            var dynamicMessage = message.ToExpandoObject();
            dynamicMessage.TryAdd("_type", type);
            var json = JsonConvert.SerializeObject(dynamicMessage);

            Provider.Send(name, json);
        }
    }
}
