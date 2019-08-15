using System;
using System.Collections.Generic;
using Hetacode.Microless.Abstractions.Managers;
using Hetacode.Microless.Abstractions.MessageBus;
using Hetacode.Microless.Abstractions.Messaging;

namespace Hetacode.Microless
{
    public class Context : IContext
    {
        private const string ACTION_KEY = "action";
        private const string ACTION_CORRELATION_ID = "correlation_id";
        private const string ACTION_VALUE = "rollback";

        private readonly IBusSubscriptions _subscription;

        public Context(IBusSubscriptions subscription)
            => _subscription = subscription;

        public Guid CorrelationId { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public bool IsRollback
        {
            get
            {
                if (Headers != null && Headers.ContainsKey(ACTION_KEY))
                {
                    return Headers[ACTION_KEY] == ACTION_VALUE;
                }
                return false;
            }
        }

        public Guid GetCorrelationIdFromHeader()
        {
            if (Headers != null && Headers.ContainsKey(ACTION_CORRELATION_ID))
            {
                return Guid.Parse(Headers[ACTION_CORRELATION_ID]);
            }

            return Guid.Empty;
        }

        public void SendError<T>(string name, T message, Dictionary<string, string> headers = null)
        {
            if (headers == null)
            {
                headers = new Dictionary<string, string>();
            }
            if (!headers.ContainsKey(ACTION_CORRELATION_ID))
            {
                headers.Add(ACTION_CORRELATION_ID, CorrelationId.ToString());
            }
            _subscription.Send(name, message, headers);
        }

        public void SendMessage<T>(string name, T message, Dictionary<string, string> headers = null)
        {
            if (headers == null)
            {
                headers = new Dictionary<string, string>();
            }
            if (!headers.ContainsKey(ACTION_CORRELATION_ID))
            {
                headers.Add(ACTION_CORRELATION_ID, CorrelationId.ToString());
            }
            _subscription.Send(name, message, headers);
        }

        public void SendRollback<T>(string name, T message)
        {
            var headers = new Dictionary<string, string>();
            headers.Add(ACTION_KEY, ACTION_VALUE);
            headers.Add(ACTION_CORRELATION_ID, CorrelationId.ToString());
            _subscription.Send(name, message, headers);
        }
    }
}
