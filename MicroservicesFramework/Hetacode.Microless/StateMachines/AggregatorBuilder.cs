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
        private Action<IContext> _initCall;
        private Action<IContext, object> _initError;

        public AggregatorBuilder(IStepsManager manager, IBusSubscriptions bus)
            => (_manager, _bus) = (manager, bus);

        public IAggregatorBuilderInitializer Init<TMessage, TError, TRollback>(Action<IContext, TMessage> response,
                                                                               Action<IContext, TError> error,
                                                                               Action<IContext, TRollback> rollbackResponse)
        {
            _manager.RegisterStep(typeof(TMessage), (c, m) =>
            {
                c.CorrelationId = Guid.NewGuid();
                response(c, (TMessage)m);
            });
            _manager.RegisterStep(typeof(TError), (c, m) => error(c, (TError)m));
            _manager.RegisterRollbackStep(typeof(TRollback), (c, m) => rollbackResponse(c, (TRollback)m));
            return this;
        }

        public IAggregatorBuilderInitializer Init<TError, TRollback>(Action<IContext> init,
                                                                     Action<IContext, TError> error,
                                                                     Action<IContext, TRollback> rollbackResponse)
        {
            _initCall = init;
            _manager.RegisterStep(typeof(TError), (c, m) => error(c, (TError)m));
            _manager.RegisterRollbackStep(typeof(TRollback), (c, m) => rollbackResponse(c, (TRollback)m));
            return this;
        }

        public IAggregatorBuilderInitializer Step<TMessage, TError, TRollback>(Action<IContext, TMessage> response,
                                                                               Action<IContext, TError> error,
                                                                               Action<IContext, TRollback> rollbackResponse)
        {
            _manager.RegisterStep(typeof(TMessage), (c, m) => response(c, (TMessage)m));
            _manager.RegisterStep(typeof(TError), (c, m) => error(c, (TError)m));
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

        public void Call(IContext context)
        {
            context.CorrelationId = Guid.NewGuid();

            _initCall(context);
        }
    }
}
