using System;
using Hetacode.Microless.Abstractions.Messaging;

namespace Hetacode.Microless.Abstractions.StateMachine
{
    public interface IAggregatorBuilder
    {
        IAggregatorBuilderInitializer Init<TMessage>(Action<IContext, TMessage> response);

        IAggregatorBuilderInitializer InitCall<TInput>(Action<IContext, TInput> init);
    }
}
