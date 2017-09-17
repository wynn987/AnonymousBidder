using System;

namespace AnonymousBidder.Data.Entity
{
    public abstract class BaseEntity
    {
        //public override bool Equals(object obj)
        //{
        //    return Equals(obj as BaseEntity);
        //}
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
