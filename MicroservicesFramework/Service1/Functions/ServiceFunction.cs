using System;
using System.Threading.Tasks;
using Contracts;
using Hetacode.Microless;
using Hetacode.Microless.Abstractions.Functions;
using Hetacode.Microless.Abstractions.Messaging;
using Hetacode.Microless.Attributes;
using Newtonsoft.Json;

namespace Service1.Functions
{
    [BindMessage(typeof(Message1Request))]
    public class ServiceFunction : IFunction<Message1Request>
    {
        public async Task Run(IContext context, Message1Request message)
        {
            Console.WriteLine($"ProcessFunction1 called : {context.CorrelationId} - {JsonConvert.SerializeObject(context.Headers)}");

            context.SendMessage("Saga", new Message1Response { Time = DateTime.Now }, context.Headers);
        }

        public async Task Rollback(IContext context, Message1Request message)
        {
            Console.WriteLine($"ProcessFunction rollback : {context.CorrelationId} - {JsonConvert.SerializeObject(context.Headers)}");
        }
    }
}
