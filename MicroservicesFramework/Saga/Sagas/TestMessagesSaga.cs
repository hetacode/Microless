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
            _states = states.Init<MessageError, MessageRequest>(c =>
            {
                var id = Guid.NewGuid();
                Console.WriteLine($"Init saga: {id}");
                c.SendMessage<MessageRequest>("Service", new MessageRequest());
            }, (c, e) =>
            {
                Console.WriteLine($"Init error id: {c.CorrelationId}");
            }, (c, e) =>
            {
                Console.WriteLine($"Rollback finished id: {c.CorrelationId}");
            })
            .Step<MessageResponse, Message1Error, Message1Request>((c, r) =>
            {
                Console.WriteLine($"Response1 saga: {c.CorrelationId}");
                c.SendMessage<Message1Request>("Service1", new Message1Request());
            }, (c, e) =>
            {
                Console.WriteLine($"Step 1 error id: {c.CorrelationId}");
            }, (c, e) =>
            {
                Console.WriteLine($"Rollback step1 id: {c.CorrelationId}");
                c.SendRollback<MessageRequest>("Service", new MessageRequest());
            })
            .Step<Message1Response, Message2Error, Message2Request>((c, r) =>
            {
                Console.WriteLine($"Response2 saga: {c.CorrelationId}");
                c.SendMessage<Message2Request>("Service2", new Message2Request());
            }, (c, e) =>
            {
                Console.WriteLine($"Step 2 error id: {c.CorrelationId}");
                c.SendRollback<Message1Request>("Service1", new Message1Request());
            }, (c, e) =>
            {
                Console.WriteLine($"Rollback step 2 id: {c.CorrelationId}");
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
