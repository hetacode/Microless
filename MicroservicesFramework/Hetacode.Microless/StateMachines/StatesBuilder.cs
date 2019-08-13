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
    public class StatesBuilder : IStatesBuilder, IStatesBuilderInitializer
    {
        private readonly IStepsManager _manager;
        private readonly IBusSubscriptions _bus;
        private Action<IContext> _initCall;

        public StatesBuilder(IStepsManager manager, IBusSubscriptions bus)
            => (_manager, _bus) = (manager, bus);

        public IStatesBuilderInitializer Init<TMessage>(Action<IContext, TMessage> response)
        {
            _manager.RegisterStep(typeof(TMessage), (c, m) => response(c, (TMessage)m));
            return this;
        }

        public IStatesBuilderInitializer Init(Action<IContext> init)
        {
            _initCall = init;
            return this;
        }

        public IStatesBuilderInitializer Step<TMessage>(Action<IContext, TMessage> response)
        {
            _manager.RegisterStep(typeof(TMessage), (c, m) => response(c, (TMessage)m));
            return this;
        }

        public IStatesBuilderInitializer Finish<TMessage>(Action<IContext, TMessage> response)
        {
            _manager.RegisterStep(typeof(TMessage), (c, m) => response(c, (TMessage)m));
            return this;
        }

        public IStatesBuilderInitializer Finish()
        {
            return this;
        }

        public void Call(IContext context)
        {
            _initCall(context);
        }
    }
}
