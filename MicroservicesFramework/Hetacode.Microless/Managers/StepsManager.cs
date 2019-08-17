using System;
using System.Collections.Generic;
using Hetacode.Microless;
using Hetacode.Microless.Abstractions.Managers;
using Hetacode.Microless.Abstractions.MessageBus;
using Hetacode.Microless.Abstractions.Messaging;
using Hetacode.Microless.Abstractions.StateMachine;
using Microsoft.Extensions.DependencyInjection;

namespace Hetacode.Microless.Managers
{
    public class StepsManager : IStepsManager
    {
        private Dictionary<Type, Action<IContext, object>> _steps = new Dictionary<Type, Action<IContext, object>>();
        private Dictionary<Type, Action<IContext, object>> _rollbackSteps = new Dictionary<Type, Action<IContext, object>>();

        private readonly IServiceProvider _services;
        private readonly IBusSubscriptions _bus;

        public StepsManager(IBusSubscriptions bus, IServiceProvider services)
            => (_bus, _services) = (bus, services);

        public Action<Context, object> Get(object message)
        {
            return _steps[message.GetType()];
        }

        public void RegisterStep(Type stepType, Action<IContext, object> action)
        {
            _steps.Add(stepType, action);
        }

        public void RegisterRollbackStep(Type stepType, Action<IContext, object> action)
        {
            _rollbackSteps.Add(stepType, action);
        }

        public void Call<TMessage>(string queueName, TMessage message, Dictionary<string, string> headers = null)
        {
            var context = new Context(_bus);
            context.Headers = headers;
            context.CorrelationId = context.GetCorrelationIdFromHeader();
            context.GetSenderFromHeader();
            context.SetSenderToHeader(queueName);

            if (context.IsRollbackDone)
            {
                _rollbackSteps[message.GetType()](context, message);
            }
            else
            {
                _steps[message.GetType()](context, message);
            }
        }

        public void InitCall<TAggregator>(string queueName, Dictionary<string, string> headers = null) where TAggregator : IAggregator
        {
            var context = new Context(_bus);
            context.Headers = headers;
            context.CorrelationId = context.GetCorrelationIdFromHeader();
            context.SetSenderToHeader(queueName);
            var aggregator = _services.GetService<TAggregator>();
            aggregator.Run(context);

        }
    }
}
