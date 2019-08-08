using System;
using System.Threading.Tasks;

namespace Hetacode.Microless.Abstractions.Filters
{
    public interface IFiltersManager
    {
        void Register<T>() where T : class, IMessageFilter;

        Task<object> ProcessIncoming(object message);

        Task<object> ProcessOutgoing(object message);
    }
}
