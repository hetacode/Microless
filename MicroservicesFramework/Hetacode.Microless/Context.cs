using System;
using System.Collections.Generic;
using Hetacode.Microless.Abstractions.Managers;
using Hetacode.Microless.Abstractions.MessageBus;
using Hetacode.Microless.Abstractions.Messaging;

namespace Hetacode.Microless
{
    public class Context : IContext
    {
        private readonly IBusSubscriptions _subscription;

        public Context(IBusSubscriptions subscription)
            => _subscription = subscription;

        public Dictionary<string, string> Headers { get; set; }

        public void SendResponse<T>(string name, T message, Dictionary<string, string> headers = null)
        {
            _subscription.Send(name, message, headers);
        }
    }
}
