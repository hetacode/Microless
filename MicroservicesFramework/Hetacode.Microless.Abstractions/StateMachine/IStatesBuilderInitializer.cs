using System;
using Hetacode.Microless;
using Hetacode.Microless.Abstractions.Messaging;

namespace Hetacode.Microless.Abstractions.StateMachine
{
    public interface IStatesBuilderInitializer
    {
        IStatesBuilderInitializer Step<TMessage>(Action<IContext, TMessage> response);

        IStatesBuilderInitializer Finish<TMessage>(Action<IContext, TMessage> response);

        IStatesBuilderInitializer Finish();

        void Call(IContext context);
    }
}
