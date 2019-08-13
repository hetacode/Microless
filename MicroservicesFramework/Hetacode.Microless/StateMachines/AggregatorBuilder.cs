using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hetacode.Microless;
using Hetacode.Microless.Abstractions.Managers;
using Hetacode.Microless.Abstractions.MessageBus;
using Hetacode.Microless.Abstractions.Messaging;
using Hetacode.Microless.Abstractions.StateMachine;
namespace Saga.StateMachine
{
    public class AggregatorBuilder : IAggregatorBuilder, IAggregatorBuilderInitializer
    {
        private readonly IStepsManager _manager;
        private readonly IBusSubscriptions _bus;
        private Action<IContext> _initCall;

        public AggregatorBuilder(IStepsManager manager, IBusSubscriptions bus)
            => (_manager, _bus) = (manager, bus);

        public IAggregatorBuilderInitializer Init<TMessage>(Action<IContext, TMessage> response)
        {
            _manager.RegisterStep(typeof(TMessage), (c, m) =>
            {
                c.CorrelationId = Guid.NewGuid();
                response(c, (TMessage)m);
            });
            return this;
        }

        public IAggregatorBuilderInitializer Init(Action<IContext> init)
        {
            _initCall = init;
            return this;
        }

        public IAggregatorBuilderInitializer Step<TMessage>(Action<IContext, TMessage> response)
        {
            _manager.RegisterStep(typeof(TMessage), (c, m) => response(c, (TMessage)m));
            return this;
        }

        public IAggregatorBuilderInitializer Finish<TMessage>(Action<IContext, TMessage> response)
        {
            _manager.RegisterStep(typeof(TMessage), (c, m) => response(c, (TMessage)m));
            return this;
        }

        public IAggregatorBuilderInitializer Finish()
        {
            return this;
        }

        public IAggregatorBuilderInitializer Rollback<TErrorMessage>(Action<IContext, TErrorMessage> response)
        {
            _manager.RegisterStep(typeof(TErrorMessage), (c, m) => response(c, (TErrorMessage)m));
            return this;
        }

        public void Call(IContext context)
        {
            context.CorrelationId = Guid.NewGuid();
            _initCall(context);
        }
    }
}
