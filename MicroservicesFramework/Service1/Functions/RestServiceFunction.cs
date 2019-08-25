using System;
using System.Threading.Tasks;
using Contracts;
using Hetacode.Microless.Abstractions.Functions;
using Hetacode.Microless.Abstractions.Messaging;
using Hetacode.Microless.Attributes;

namespace Service1.Functions
{
    [HttpRequest("/service1", "Service", Hetacode.Microless.Enums.HttpMethod.POST, typeof(MessageRequest))]
    public class RestServiceFunction : IFunction<MessageRequest>
    {
        public RestServiceFunction()
        {
        }

        public async Task Run(IContext context, MessageRequest message)
        {
            Console.WriteLine("Rest call!!!");
            context.HttpResponse(new { Message = "Rest response" }, context.Headers);
        }

        public Task Rollback(IContext context, MessageRequest message)
        {
            throw new NotImplementedException();
        }


    }
}
