using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hetacode.Microless.Abstractions.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Hetacode.Microless.Managers
{
    public class FiltersManager : IFiltersManager
    {
        private List<Type> _filters = new List<Type>();
        private IServiceProvider _services;
        private IServiceCollection _container;

        public FiltersManager(IServiceProvider services, IServiceCollection container)
            => (_services, _container) = (services, container);

        public void Register<T>() where T : class, IMessageFilter
        {
            _container.AddTransient<T>();
            _filters.Add(typeof(T));
        }

        public async Task<object> ProcessIncoming(object message)
        {
            using (var scope = _services.CreateScope())
            {
                foreach (var filterType in _filters)
                {
                    var filter = scope.ServiceProvider.GetService(filterType) as IMessageFilter;
                    message = await filter.Incoming(message);
                }
            }

            return message;
        }

        public async Task<object> ProcessOutgoing(object message)
        {
            using (var scope = _services.CreateScope())
            {
                foreach (var filterType in _filters)
                {
                    var filter = scope.ServiceProvider.GetService(filterType) as IMessageFilter;
                    message = await filter.OutGoing(message);
                }
            }

            return message;
        }
    }
}
