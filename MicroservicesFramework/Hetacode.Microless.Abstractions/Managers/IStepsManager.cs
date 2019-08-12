using System;
using Hetacode.Microless.Abstractions.Messaging;

namespace Hetacode.Microless.Abstractions.Managers
{
    public interface IStepsManager
    {
        void RegisterStep(Type stepType, Action<IContext, object> action);

        Action<IContext, TMessage> Get<TMessage>(TMessage message);
    }
}
