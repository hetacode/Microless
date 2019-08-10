using System;
using System.Threading.Tasks;
using Contracts;
using Hetacode.Microless;
using Hetacode.Microless.Attributes;
using Hetacode.Microless.MessageBus;
using Newtonsoft.Json;

namespace Service.Functions
{
    public class ProcessFunction
    {
        [BindMessage(typeof(MessageRequest))]
        public async Task Run(Context context, MessageRequest message)
        {
            Console.WriteLine($"ProcessFunction called : {message.CorrelationId} - {JsonConvert.SerializeObject(context.Headers)}");

            context.SendResponse("Saga", new MessageResponse { CorrelationId = message.CorrelationId, Time = DateTime.Now }, context.Headers);
        }
    }
}
