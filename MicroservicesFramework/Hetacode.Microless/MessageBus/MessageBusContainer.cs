using System;
using System.Collections.Generic;
using Hetacode.Microless.Abstractions.MessageBus;

namespace Hetacode.Microless.MessageBus
{
    public class MessageBusContainer
    {
        private readonly IBusSubscriptions _configuration;

        public MessageBusContainer(IBusSubscriptions configuration) => _configuration = configuration;

        public void Send(string name, object message, Dictionary<string, string> headers = null)
        {
            _configuration.Send(name, message, headers);
        }
    }
}
