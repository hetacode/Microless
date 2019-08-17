using System;
using Hetacode.Microless;
using Hetacode.Microless.Abstractions.Messaging;

namespace Hetacode.Microless.Abstractions.StateMachine
{
    public interface IAggregatorBuilderInitializer
    {
        IAggregatorBuilderInitializer Step<TMessage, TError, TRollback>(Action<IContext, TMessage> response, Action<IContext, TError> error, Action<IContext, TRollback> rollbackResponse);

        IAggregatorBuilderInitializer Finish<TMessage>(Action<IContext, TMessage> response);

        IAggregatorBuilderInitializer Finish();

        void Call(IContext context);
    }
}
