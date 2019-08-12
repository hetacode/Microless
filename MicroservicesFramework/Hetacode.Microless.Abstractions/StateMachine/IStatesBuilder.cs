using System;
using Hetacode.Microless.Abstractions.Messaging;

namespace Hetacode.Microless.Abstractions.StateMachine
{
    public interface IStatesBuilder
    {
        IStatesBuilderInitializer Init();

        IStatesBuilderInitializer Init(Action<IContext> init);
    }
}
