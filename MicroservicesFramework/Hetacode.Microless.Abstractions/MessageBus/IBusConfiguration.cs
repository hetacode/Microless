using System;
namespace Hetacode.Microless.Abstractions.MessageBus
{
    public interface IBusConfiguration
    {
        IQueueProvider Provider { get; set; }

        void AddReceiver(string name, Action<object> messageCallback);

        void Send(string name, object message);
    }
}
