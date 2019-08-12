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

        public StatesBuilder(IStepsManager manager, IBusSubscriptions bus)
            => (_manager, _bus) = (manager, bus);

        public IStatesBuilderInitializer Init()
        {
            return this;
        }

        public IStatesBuilderInitializer Init(Action<IContext> init)
        {
            //init.Invoke(new Context(_bus));
            return this;
        }

        public IStatesBuilderInitializer Step<TMessage>(Action<IContext, TMessage> response)
        {
            _manager.RegisterStep(typeof(TMessage), (c, m) => response(c, (TMessage)m));
            return this;
        }

        public void Finish<TMessage>(Action<IContext, TMessage> response)
        {
            _manager.RegisterStep(typeof(TMessage), (c, m) => response(c, (TMessage)m));
        }

        public void Finish()
        {
        }
    }
}
