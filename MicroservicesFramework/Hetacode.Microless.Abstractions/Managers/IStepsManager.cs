using System;
using System.Collections.Generic;
using Hetacode.Microless.Abstractions.Messaging;
using Hetacode.Microless.Abstractions.StateMachine;

namespace Hetacode.Microless.Abstractions.Managers
{
    public interface IStepsManager
    {
        void RegisterStep(Type stepType, Action<IContext, object> action);

        void RegisterRollbackStep(Type stepType, Action<IContext, object> action);

        void Call<TMessage>(string queueName, TMessage message, Dictionary<string, string> headers = null);

        void InitCall<TAggregator>(string queueName, Dictionary<string, string> headers = null) where TAggregator : IAggregator;
    }
}
