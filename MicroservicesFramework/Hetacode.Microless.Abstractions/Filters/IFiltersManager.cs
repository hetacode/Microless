using System;
namespace Hetacode.Microless.Abstractions.Filters
{
    public interface IFiltersManager
    {
        void Register<T>() where T : class, IMessageFilter;
    }
}
