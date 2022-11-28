using System.Collections.Generic;

namespace RaiseEventBus.Runtime.Models.Comparers
{
    public class NetworkPlayerComparer : IEqualityComparer<NetworkPlayer>
    {
        public static NetworkPlayerComparer GetComparer()
        {
            return new NetworkPlayerComparer();
        }

        public bool Equals(NetworkPlayer x, NetworkPlayer y)
        {
            return x.OwnerID.Equals(y.OwnerID);
        }

        public int GetHashCode(NetworkPlayer obj)
        {
            return obj.OwnerID;
        }
    }
}