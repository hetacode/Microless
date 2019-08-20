using System;
using Hetacode.Microless.Abstractions.Messaging;

namespace Hetacode.Microless.Abstractions.StateMachine
{
    public interface IAggregator<TInput>
    {
        //void Run(IContext context);

        void Run(IContext context, TInput input);
    }
}
