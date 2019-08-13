using System;
using Hetacode.Microless.Abstractions.Messaging;

namespace Hetacode.Microless.Abstractions.StateMachine
{
    public interface IStatesBuilder
    {
        IStatesBuilderInitializer Init<TMessage>(Action<IContext, TMessage> response);

        IStatesBuilderInitializer Init(Action<IContext> init);
    }
}
