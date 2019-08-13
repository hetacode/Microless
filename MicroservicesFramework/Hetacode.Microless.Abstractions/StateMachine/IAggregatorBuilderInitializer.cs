﻿using System;
using Hetacode.Microless;
using Hetacode.Microless.Abstractions.Messaging;

namespace Hetacode.Microless.Abstractions.StateMachine
{
    public interface IAggregatorBuilderInitializer
    {
        IAggregatorBuilderInitializer Step<TMessage>(Action<IContext, TMessage> response);

        IAggregatorBuilderInitializer Finish<TMessage>(Action<IContext, TMessage> response);

        IAggregatorBuilderInitializer Finish();

        IAggregatorBuilderInitializer Rollback<TErrorMessage>(Action<IContext, TErrorMessage> response);

        void Call(IContext context);
    }
}
