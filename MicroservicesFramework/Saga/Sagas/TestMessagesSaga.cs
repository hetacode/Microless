using System;
using Contracts;
using Hetacode.Microless.Abstractions.Messaging;
using Hetacode.Microless.Abstractions.StateMachine;
using Hetacode.Microless.Attributes;
using Saga.StateMachine;

namespace Saga.Sagas
{
    public class TestMessagesSaga : IAggregator
    {
        private readonly IAggregatorBuilderInitializer _states;

        public TestMessagesSaga(IAggregatorBuilder states)
        {
            _states = states.Init(c =>
            {
                var id = Guid.NewGuid();
                Console.WriteLine($"Init saga: {id}");
                c.SendResponse<MessageRequest>("Service", new MessageRequest());
            })
            .Step<MessageResponse>((c, r) =>
            {
                Console.WriteLine($"Response1 saga: {c.CorrelationId}");
                c.SendResponse<Message1Request>("Service1", new Message1Request());
            })
            .Step<Message1Response>((c, r) =>
            {
                Console.WriteLine($"Response2 saga: {c.CorrelationId}");
                c.SendResponse<Message2Request>("Service2", new Message2Request());
            })
            .Finish<Message2Response>((c, r) =>
            {
                Console.WriteLine($"Finish saga: {c.CorrelationId}");
            });
        }

        public void Run(IContext context)
        {
            _states.Call(context);
        }
    }
}
