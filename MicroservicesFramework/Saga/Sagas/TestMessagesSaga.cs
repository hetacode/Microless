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
        private readonly IStatesBuilderInitializer _states;

        public TestMessagesSaga(IStatesBuilder states)
        {
            _states = states.Init(c =>
            {
                var id = Guid.NewGuid();
                Console.WriteLine($"Init saga: {id}");
                c.SendResponse<MessageRequest>("Service", new MessageRequest { CorrelationId = id });
            })
            .Step<MessageResponse>((c, r) =>
            {
                Console.WriteLine($"Response saga: {r.CorrelationId}");
            });
        }

        public void Run(IContext context)
        {
            _states.Call(context);
        }
    }
}
