using System;
using System.Collections.Generic;
using Hetacode.Microless;
using Hetacode.Microless.Abstractions.Managers;
using Hetacode.Microless.Abstractions.Messaging;

namespace Hetacode.Microless.Managers
{
    public class StepsManager : IStepsManager
    {
        private Dictionary<Type, Action<IContext, object>> _steps = new Dictionary<Type, Action<IContext, object>>();

        public Action<Context, object> Get(object message)
        {
            return _steps[message.GetType()];
        }

        public void RegisterStep(Type stepType, Action<IContext, object> action)
        {
            _steps.Add(stepType, action);
        }

        public Action<IContext, TMessage> Get<TMessage>(TMessage message)
        {
            return new Action<IContext, TMessage>((c, m) => _steps[typeof(TMessage)](c, m));
        }
    }
}
