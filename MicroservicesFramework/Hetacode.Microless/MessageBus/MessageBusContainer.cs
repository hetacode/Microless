using System;
using Hetacode.Microless.Abstractions.MessageBus;

namespace Hetacode.Microless.MessageBus
{
    public class MessageBusContainer
    {
        private readonly IBusConfiguration _configuration;

        public MessageBusContainer(IBusConfiguration configuration) => _configuration = configuration;

        public void Send(string name, object message)
        {
            _configuration.Send(name, message);
        }
    }
}
