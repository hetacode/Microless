using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hetacode.Microless.Abstractions.MessageBus
{
    public interface IQueueProvider
    {
        void Init();

        void AddReceiver(string name, Action<string, Dictionary<string, string>> messageCallback);

        void Send(string name, string jsonMessage, Dictionary<string, string> headers = null);

    }
}
