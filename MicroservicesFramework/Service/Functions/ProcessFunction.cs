using System;
using System.Threading.Tasks;
using Contracts;
using Hetacode.Microless;
using Hetacode.Microless.Attributes;
using Hetacode.Microless.MessageBus;
using Newtonsoft.Json;

namespace Service.Functions
{
    [BindMessage(typeof(MessageRequest))]
    public class ProcessFunction
    {
        public async Task Run(Context context, MessageRequest message)
        {
            Console.WriteLine($"ProcessFunction called : {context.CorrelationId} - {JsonConvert.SerializeObject(context.Headers)}");

            context.SendMessage("Saga", new MessageResponse { Time = DateTime.Now }, context.Headers);
        }

        public async Task Rollback(Context context, MessageRequest message)
        {
            Console.WriteLine($"ProcessFunction rollback : {context.CorrelationId} - {JsonConvert.SerializeObject(context.Headers)}");
        }
    }
}
