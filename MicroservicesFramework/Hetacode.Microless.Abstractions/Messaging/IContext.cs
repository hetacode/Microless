using System;
using System.Collections.Generic;

namespace Hetacode.Microless.Abstractions.Messaging
{
    public interface IContext
    {
        string Sender { get; }

        Guid CorrelationId { get; set; }

        Dictionary<string, string> Headers { get; set; }

        bool IsRollback { get; }

        bool IsRollbackDone { get; }

        void SendMessage<T>(string name, T message, Dictionary<string, string> headers = null);

        void SendError<T>(string name, T message, Dictionary<string, string> headers = null);

        void SendRollback<T>(string name, T message);

        void HttpResponse<T>(T message, Dictionary<string, string> headers = null);
    }
}
