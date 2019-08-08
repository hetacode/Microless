using System;
using Hetacode.Microless.Abstractions.Filters;

namespace Hetacode.Microless.Abstractions.MessageBus
{
    public interface IBusConfiguration
    {
        IQueueProvider Provider { get; set; }

        IFiltersManager Filters { get; }
    }
}
