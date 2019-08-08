using System;
using System.Threading.Tasks;

namespace Hetacode.Microless.Abstractions.Filters
{
    public interface IMessageFilter
    {
        Task<object> Incoming(object message);

        Task<object> OutGoing(object message);
    }
}
