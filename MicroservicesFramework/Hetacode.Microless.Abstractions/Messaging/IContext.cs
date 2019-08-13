using System;
using System.Collections.Generic;

namespace Hetacode.Microless.Abstractions.Messaging
{
    public interface IContext
    {
        Dictionary<string, string> Headers { get; set; }

        void SendResponse<T>(string name, T message, Dictionary<string, string> headers = null);
    }
}
