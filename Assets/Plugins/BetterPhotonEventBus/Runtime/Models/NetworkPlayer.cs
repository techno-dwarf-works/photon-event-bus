using System;

namespace BetterPhotonEventBus.Models
{
    [Serializable]
    public readonly struct NetworkPlayer
    {
        private readonly int _ownerID;

        public NetworkPlayer(int ownerID, bool isLocal, string nickname, bool isHost, CustomProperties customProperties)
        {
            _ownerID = ownerID;
            IsLocal = isLocal;
            Nickname = nickname;
            CustomProperties = customProperties;
            IsHost = isHost;
            IsValid = true;
        }

        public bool IsValid { get; }

        public CustomProperties CustomProperties { get; }

        public int OwnerID => IsValid ? _ownerID : -1;

        public bool IsHost { get; }

        public bool IsLocal { get; }

        public string Nickname { get; }

        public bool Equals(NetworkPlayer networkPlayer)
        {
            return networkPlayer.OwnerID.Equals(OwnerID);
        }
    }
}