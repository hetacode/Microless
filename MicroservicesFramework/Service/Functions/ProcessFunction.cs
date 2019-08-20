using System;
using System.Threading.Tasks;
using Contracts;
using Hetacode.Microless;
using Hetacode.Microless.Abstractions.Functions;
using Hetacode.Microless.Abstractions.Messaging;
using Hetacode.Microless.Attributes;
using Hetacode.Microless.MessageBus;
using Newtonsoft.Json;

namespace Service.Functions
{
    [BindMessage(typeof(MessageRequest))]
    public class ProcessFunction : IFunction<MessageRequest>
    {
        public async Task Run(IContext context, MessageRequest message)
        {
            Console.WriteLine($"ProcessFunction called : {context.CorrelationId} - {JsonConvert.SerializeObject(context.Headers)}");

            context.SendMessage("Saga", new MessageResponse { Time = DateTime.Now }, context.Headers);
        }

        public async Task Rollback(IContext context, MessageRequest message)
        {
            Console.WriteLine($"ProcessFunction rollback : {context.CorrelationId} - {JsonConvert.SerializeObject(context.Headers)}");
        }
    }
}
