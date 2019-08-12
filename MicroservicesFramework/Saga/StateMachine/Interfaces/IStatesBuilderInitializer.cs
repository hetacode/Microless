using System;
using Hetacode.Microless;

namespace Saga.StateMachine.Interfaces
{
    public interface IStatesBuilderInitializer
    {
        IStatesBuilderInitializer Step<TMessage>(Action<Context, TMessage> response);

        void Finish<TMessage>(Action<Context, TMessage> response);

        void Finish();
    }
}
