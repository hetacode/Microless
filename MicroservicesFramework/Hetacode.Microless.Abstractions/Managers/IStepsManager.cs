using System;
using System.Collections.Generic;
using Hetacode.Microless.Abstractions.Messaging;

namespace Hetacode.Microless.Abstractions.Managers
{
    public interface IStepsManager
    {
        void RegisterStep(Type stepType, Action<IContext, object> action);

        void Call<TMessage>(TMessage message, Dictionary<string, string> headers = null);
    }
}
