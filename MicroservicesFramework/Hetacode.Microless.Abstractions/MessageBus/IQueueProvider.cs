using System;
using System.Threading.Tasks;

namespace Hetacode.Microless.Abstractions.MessageBus
{
    public interface IQueueProvider
    {
        void Init();

        void AddReceiver(string name, Action<string> messageCallback);

        void Send(string name, string jsonMessage);

    }
}
