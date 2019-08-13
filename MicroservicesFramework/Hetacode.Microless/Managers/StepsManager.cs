using System;
using System.Collections.Generic;
using Hetacode.Microless;
using Hetacode.Microless.Abstractions.Managers;
using Hetacode.Microless.Abstractions.MessageBus;
using Hetacode.Microless.Abstractions.Messaging;

namespace Hetacode.Microless.Managers
{
    public class StepsManager : IStepsManager
    {
        private Dictionary<Type, Action<IContext, object>> _steps = new Dictionary<Type, Action<IContext, object>>();
        private readonly IBusSubscriptions _bus;

        public StepsManager(IBusSubscriptions bus)
            => _bus = bus;

        public Action<Context, object> Get(object message)
        {
            return _steps[message.GetType()];
        }

        public void RegisterStep(Type stepType, Action<IContext, object> action)
        {
            _steps.Add(stepType, action);
        }

        public void Call<TMessage>(TMessage message, Dictionary<string, string> headers = null)
        {
            var context = new Context(_bus);
            context.Headers = headers;
            _steps[typeof(TMessage)](context, message);
        }
    }
}
