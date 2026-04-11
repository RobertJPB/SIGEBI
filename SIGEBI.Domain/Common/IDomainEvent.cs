using System;

namespace SIGEBI.Domain.Common
{
    /// <summary>
    /// Contract for all business events that occur in the domain.
    /// </summary>
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
