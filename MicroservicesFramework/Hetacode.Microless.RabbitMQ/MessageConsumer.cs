using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Hetacode.Microless.RabbitMQ
{
    public class MessageConsumer : IBasicConsumer
    {
        private Action<string, Dictionary<string, string>> _callback;

        public MessageConsumer(Action<string, Dictionary<string, string>> callback) => _callback = callback;

        public IModel Model { get; }


        public event EventHandler<ConsumerEventArgs> ConsumerCancelled;


        public void HandleBasicCancel(string consumerTag)
        {
        }

        public void HandleBasicCancelOk(string consumerTag)
        {
        }

        public void HandleBasicConsumeOk(string consumerTag)
        {
        }

        public void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            Dictionary<string, string> headers = null;
            if (properties.Headers != null && properties.Headers.Any())
            {
                headers = properties.Headers
                                    .ToDictionary(d => d.Key, d => Encoding.UTF8.GetString(d.Value as byte[]));
            }
            var json = Encoding.UTF8.GetString(body);
            _callback(json, headers);
        }

        public void HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
        }
    }
}
