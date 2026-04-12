using System;
using System.Collections.Generic;
using System.Linq;

namespace SIGEBI.Domain.Common
{
    public abstract class BaseValueObject : IEquatable<BaseValueObject>
    {
        protected abstract IEnumerable<object?> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var other = (BaseValueObject)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x?.GetHashCode() ?? 0)
                .Aggregate((x, y) => x ^ y);
        }

        public bool Equals(BaseValueObject? other)
        {
            return Equals((object?)other);
        }

        public static bool operator ==(BaseValueObject? left, BaseValueObject? right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
                return true;

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(BaseValueObject? left, BaseValueObject? right)
        {
            return !(left == right);
        }
    }
}

