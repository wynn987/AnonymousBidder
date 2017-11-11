using System;

namespace AnonymousBidder.Data.Entity
{
    public abstract class BaseEntity
    {
        private Type GetUnproxiedType()
        {
            return GetType();
        }

        public static bool operator ==(BaseEntity x, BaseEntity y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(BaseEntity x, BaseEntity y)
        {
            return !(x == y);
        }
    }
}
