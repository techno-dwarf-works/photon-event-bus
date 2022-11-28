using System.Collections.Generic;
using RaiseEventBus.Runtime.Models.EventTypes;

namespace RaiseEventBus.Runtime.Models.Comparers
{
    public class EventCodeTypeComparer : IEqualityComparer<EventCodeType>
    {
        public bool Equals(EventCodeType x, EventCodeType y)
        {
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Type == y.Type && x.EventCode == y.EventCode;
        }

        public int GetHashCode(EventCodeType obj)
        {
            unchecked
            {
                return ((obj.Type != null ? obj.Type.GetHashCode() : 0) * 397) ^ obj.EventCode.GetHashCode();
            }
        }
    }
}