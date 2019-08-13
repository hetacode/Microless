using System;
using Hetacode.Microless.Abstractions.Messaging;

namespace Hetacode.Microless.Abstractions.StateMachine
{
    public interface IAggregator
    {
        void Run(IContext context);
    }
}
