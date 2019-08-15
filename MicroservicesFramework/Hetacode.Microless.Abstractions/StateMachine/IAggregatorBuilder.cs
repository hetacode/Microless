using System;
using Hetacode.Microless.Abstractions.Messaging;

namespace Hetacode.Microless.Abstractions.StateMachine
{
    public interface IAggregatorBuilder
    {
        IAggregatorBuilderInitializer Init<TMessage, TError>(Action<IContext, TMessage> response, Action<IContext, TError> error);

        IAggregatorBuilderInitializer Init<TError>(Action<IContext> init, Action<IContext, TError> error);
    }
}
