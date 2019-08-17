using System;
using Hetacode.Microless.Abstractions.Messaging;

namespace Hetacode.Microless.Abstractions.StateMachine
{
    public interface IAggregatorBuilder
    {
        IAggregatorBuilderInitializer Init<TMessage, TError, TRollback>(Action<IContext, TMessage> response, Action<IContext, TError> error, Action<IContext, TRollback> rollbackResponse);

        IAggregatorBuilderInitializer Init<TError, TRollback>(Action<IContext> init, Action<IContext, TError> error, Action<IContext, TRollback> rollbackResponse);
    }
}
