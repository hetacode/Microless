using System;
using Contracts;
using Hetacode.Microless.Abstractions.Messaging;
using Hetacode.Microless.Abstractions.StateMachine;
using Hetacode.Microless.Attributes;
using Hetacode.Microless.Enums;
using Saga.StateMachine;

namespace Saga.Sagas
{
    [HttpRequest("/test", "Saga", HttpMethod.POST, typeof(MessageRequest))]
    [Aggregator]
    public class TestMessagesSaga : IAggregator<MessageRequest>
    {
        private readonly IAggregatorBuilderInitializer _states;

        public TestMessagesSaga(IAggregatorBuilder states)
        {
            _states = states.InitCall<MessageRequest>((c, i) =>
            {
                var id = Guid.NewGuid();

                Console.WriteLine($"Init saga: {id} - Input: {i}");
                c.SendMessage<MessageRequest>("Service", i);
            })
            .Error<MessageError>((c, e) =>
            {
                Console.WriteLine($"Init error id: {c.CorrelationId}");
            })
            .Rollback<MessageRequest>((c, e) =>
            {
                Console.WriteLine($"Rollback finished id: {c.CorrelationId}");
            })
            .Step<MessageResponse>((c, r) =>
            {
                Console.WriteLine($"Response1 saga: {c.CorrelationId}");
                c.SendMessage<Message1Request>("Service1", new Message1Request());
            })
            .Error<Message1Error>((c, e) =>
            {
                Console.WriteLine($"Step 1 error id: {c.CorrelationId}");
            })
            .Rollback<Message1Request>((c, e) =>
            {
                Console.WriteLine($"Rollback step1 id: {c.CorrelationId}");
                c.SendRollback<MessageRequest>("Service", new MessageRequest());
            })
            .Step<Message1Response>((c, r) =>
            {
                Console.WriteLine($"Response2 saga: {c.CorrelationId}");
                c.SendMessage<Message2Request>("Service2", new Message2Request());
            })
            .Error<Message2Error>((c, e) =>
            {
                Console.WriteLine($"Step 2 error id: {c.CorrelationId}");
                c.SendRollback<Message1Request>("Service1", new Message1Request());
            })
            .Rollback<Message2Request>((c, e) =>
            {
                Console.WriteLine($"Rollback step 2 id: {c.CorrelationId}");
            })
            .Finish<Message2Response>((c, r) =>
            {
                Console.WriteLine($"Finish saga: {c.CorrelationId}");
            });
        }

        public void Run(IContext context, MessageRequest input)
        {
            _states.Call(context, input);
        }
    }
}
