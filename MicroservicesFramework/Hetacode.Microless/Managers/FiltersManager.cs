using System;
using System.Collections.Generic;
using Hetacode.Microless.Abstractions.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Hetacode.Microless.Managers
{
    public class FiltersManager : IFiltersManager
    {
        private IServiceCollection _services;
        private List<Type> _filters = new List<Type>();

        public FiltersManager(IServiceCollection services)
            => _services = services;


        public void Register<T>() where T : class, IMessageFilter
        {
            _services.AddTransient<T>();
            _filters.Add(typeof(T));
        }
    }
}
