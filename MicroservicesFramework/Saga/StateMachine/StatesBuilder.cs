using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hetacode.Microless;
using Hetacode.Microless.Abstractions.MessageBus;
using Saga.StateMachine.Interfaces;
using Saga.StateMachine.Managers;

namespace Saga.StateMachine
{
    public class StatesBuilder
    {
        private readonly StepsManager _manager;
        private readonly IBusSubscriptions _bus;

        public StatesBuilder(StepsManager manager, IBusSubscriptions bus)
            => (_manager, _bus) = (manager, bus);

        public IStatesBuilderInitializer Init()
        {
            return new StatesBuilderInitializer(_manager); // TODO: move to container
        }

        public IStatesBuilderInitializer Init(Action<Context> init)
        {
            init.Invoke(new Context(_bus));
            return new StatesBuilderInitializer(_manager);
        }
    }

    public class StatesBuilderInitializer : IStatesBuilderInitializer
    {
        private readonly StepsManager _manager;

        public StatesBuilderInitializer(StepsManager manager)
            => _manager = manager;

        public IStatesBuilderInitializer Step<TMessage>(Action<Context, TMessage> response)
        {
            _manager.RegisterStep(typeof(TMessage), (c, m) => response(c, (TMessage)m));
            return this;
        }

        public void Finish<TMessage>(Action<Context, TMessage> response)
        {
            _manager.RegisterStep(typeof(TMessage), (c, m) => response(c, (TMessage)m));
        }

        public void Finish()
        {
        }
    }
}
