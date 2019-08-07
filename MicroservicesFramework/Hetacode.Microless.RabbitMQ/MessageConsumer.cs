using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Hetacode.Microless.RabbitMQ
{
    public class MessageConsumer : IBasicConsumer
    {
        private Action<string> _callback;

        public MessageConsumer(Action<string> callback) => _callback = callback;

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
            var json = Encoding.UTF8.GetString(body);
            _callback(json);
        }

        public void HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
        }
    }
}
