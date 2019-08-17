using System;
using System.Threading.Tasks;
using Contracts;
using Hetacode.Microless;
using Hetacode.Microless.Attributes;
using Newtonsoft.Json;

namespace Service1.Functions
{
    [BindMessage(typeof(Message1Request))]
    public class ServiceFunction
    {
        public async Task Run(Context context, Message1Request message)
        {
            Console.WriteLine($"ProcessFunction1 called : {context.CorrelationId} - {JsonConvert.SerializeObject(context.Headers)}");

            context.SendMessage("Saga", new Message1Response { Time = DateTime.Now }, context.Headers);
        }

        public async Task Rollback(Context context, Message1Request message)
        {
            Console.WriteLine($"ProcessFunction rollback : {context.CorrelationId} - {JsonConvert.SerializeObject(context.Headers)}");
        }
    }
}
