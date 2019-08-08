using System;
namespace Hetacode.Microless.Abstractions.MessageBus
{
    public interface IBusSubscriptions
    {
        void AddReceiver(string name, Action<object> messageCallback);

        void Send(string name, object message);
    }
}
