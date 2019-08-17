using System;
using System.Collections.Generic;

namespace Hetacode.Microless.Abstractions.MessageBus
{
    public interface IBusSubscriptions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Receiver Queue name</param>
        /// <param name="messageCallback">
        /// string - Receiver Queue name
        /// object - message from publisher
        /// Dictionary - headers
        /// </param>
        void AddReceiver(string name, Action<string, object, Dictionary<string, string>> messageCallback);

        void Send(string name, object message, Dictionary<string, string> headers = null);
    }
}
