using System;
using System.Threading.Tasks;
using Contracts;
using Hetacode.Microless;
using Hetacode.Microless.Attributes;
using Hetacode.Microless.Exceptions;
using Newtonsoft.Json;

namespace Service2.Functions
{
    [BindMessage(typeof(Message2Request))]
    public class ServiceFunction
    {
        public async Task Run(Context context, Message2Request message)
        {
            Console.WriteLine($"ProcessFunction2 called : {context.CorrelationId} - {JsonConvert.SerializeObject(context.Headers)}");
            context.SendError("Saga", new Message2Error());
            //context.SendMessage("Saga", new Message2Response { Time = DateTime.Now }, context.Headers);
        }

        public async Task Rollback(Context context, Message2Request message)
        {
            Console.WriteLine($"ProcessFunction rollback : {context.CorrelationId} - {JsonConvert.SerializeObject(context.Headers)}");
        }
    }
}
