using System;
using Contracts;
using Saga.StateMachine;

namespace Saga.Sagas
{
    public class TestMessagesSaga
    {
        public TestMessagesSaga(StatesBuilder states)
        {
            states.Init(c =>
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
    }
}
