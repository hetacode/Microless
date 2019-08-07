using System;
using System.Collections.Generic;
using System.Text;
using Hetacode.Microless.Abstractions.MessageBus;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Hetacode.Microless.RabbitMQ
{
    public class RabbitMQProvider : IQueueProvider
    {
        private readonly string _host;
        private readonly string _username;
        private readonly string _password;
        private readonly string _virtualHost;
        private readonly int _port;
        private ConnectionFactory _factory;
        private Dictionary<string, IModel> _channels = new Dictionary<string, IModel>();

        public RabbitMQProvider(string host, string username, string password, string virtualHost, int port = 5672)
            => (_host, _username, _password, _virtualHost, _port) = (host, username, password, virtualHost, port);

        public void Init()
        {
            _factory = new ConnectionFactory
            {
                HostName = _host,
                VirtualHost = _virtualHost,
                UserName = _username,
                Password = _password
            };

        }

        public void AddReceiver(string name, Action<string> messageCallback)
        {
            IModel channel;
            if (!_channels.ContainsKey(name))
            {
                channel = _factory.CreateConnection().CreateModel();
                channel.QueueDeclare(name, false, false, true, null);
            }
            else
            {
                channel = _channels[name];
            }
            channel.BasicConsume(name, true, new MessageConsumer(messageCallback));
            _channels[name] = channel;
        }

        public void Send(string name, string jsonMessage)
        {
            IModel channel;
            if (!_channels.ContainsKey(name))
            {
                channel = _factory.CreateConnection().CreateModel();
                channel.QueueDeclare(name, false, false, true, null);
            }
            else
            {
                channel = _channels[name];
            }

            var body = Encoding.UTF8.GetBytes(jsonMessage);
            channel.BasicPublish("", name, null, body);
        }
    }
}
