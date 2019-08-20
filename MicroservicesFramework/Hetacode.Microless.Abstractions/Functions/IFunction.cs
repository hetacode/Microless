using System;
using System.Threading.Tasks;
using Hetacode.Microless.Abstractions.Messaging;

namespace Hetacode.Microless.Abstractions.Functions
{
    public interface IFunction<TMessageRequest>
    {
        Task Run(IContext context, TMessageRequest message);

        Task Rollback(IContext context, TMessageRequest message);
    }
}
