using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hetacode.Microless;
using Hetacode.Microless.Abstractions.Managers;
using Hetacode.Microless.Abstractions.MessageBus;
using Hetacode.Microless.Abstractions.Messaging;
using Hetacode.Microless.Abstractions.StateMachine;
using Hetacode.Microless.Exceptions;

namespace Saga.StateMachine
{
    public class AggregatorBuilder : IAggregatorBuilder, IAggregatorBuilderInitializer
    {
        private readonly IStepsManager _manager;
        private readonly IBusSubscriptions _bus;
        private Action<IContext, object> _initCall;
        private Action<IContext, object> _initError;

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

        public IAggregatorBuilderInitializer InitCall<TInput>(Action<IContext, TInput> init)
        {
            _initCall = (c, a) => init(c, (TInput)a);
            return this;
        }

        public IAggregatorBuilderInitializer Step<TMessage>(Action<IContext, TMessage> response)
        {
            _manager.RegisterStep(typeof(TMessage), (c, m) => response(c, (TMessage)m));
            return this;
        }

        public IAggregatorBuilderInitializer Error<TError>(Action<IContext, TError> error)
        {
            _manager.RegisterStep(typeof(TError), (c, m) => error(c, (TError)m));
            return this;
        }

        public IAggregatorBuilderInitializer Rollback<TRollback>(Action<IContext, TRollback> rollbackResponse)
        {
            _manager.RegisterRollbackStep(typeof(TRollback), (c, m) => rollbackResponse(c, (TRollback)m));
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

        public void Call<TInput>(IContext context, TInput input)
        {
            context.CorrelationId = Guid.NewGuid();

            _initCall(context, input);
        }


    }
}
