using System;
using System.Collections.Generic;

namespace Hetacode.Microless.Abstractions.Messaging
{
    public interface IContext
    {
        Guid CorrelationId { get; set; }

        Dictionary<string, string> Headers { get; set; }

        bool IsRollback { get; }

        void SendResponse<T>(string name, T message, Dictionary<string, string> headers = null);

        void SendRollback<T>(string name, T message);
    }
}
