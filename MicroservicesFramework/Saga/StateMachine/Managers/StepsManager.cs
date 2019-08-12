using System;
using System.Collections.Generic;
using Hetacode.Microless;

namespace Saga.StateMachine.Managers
{
    public class StepsManager
    {
        private Dictionary<Type, Action<Context, object>> _steps = new Dictionary<Type, Action<Context, object>>();

        public void RegisterStep(Type stepType, Action<Context, object> action)
        {
            _steps.Add(stepType, action);
        }

        public Action<Context, object> Get(object message)
        {
            return _steps[message.GetType()];
        }
    }
}
