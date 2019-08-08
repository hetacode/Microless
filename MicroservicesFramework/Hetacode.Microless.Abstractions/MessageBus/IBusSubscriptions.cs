using System;
using System.Collections.Generic;

namespace Hetacode.Microless.Abstractions.MessageBus
{
    public interface IBusSubscriptions
    {
        void AddReceiver(string name, Action<object, Dictionary<string, string>> messageCallback);

        void Send(string name, object message, Dictionary<string, string> headers = null);
    }
}
