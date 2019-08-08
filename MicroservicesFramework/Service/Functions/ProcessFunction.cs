using System;
using System.Threading.Tasks;
using Contracts;
using Hetacode.Microless.Attributes;
using Hetacode.Microless.Core;
using Hetacode.Microless.MessageBus;

namespace Service.Functions
{
    public class ProcessFunction
    {
        private readonly MessageBusContainer _bus;

        public ProcessFunction(MessageBusContainer bus)
            => _bus = bus;

        [BindMessage(typeof(MessageRequest))]
        public async Task Run(Context context, MessageRequest message)
        {
            Console.WriteLine($"ProcessFunction called : ${message.CorrelationId}");

            _bus.Send("Saga", new MessageResponse { CorrelationId = message.CorrelationId, Time = DateTime.Now });
        }
    }
}
