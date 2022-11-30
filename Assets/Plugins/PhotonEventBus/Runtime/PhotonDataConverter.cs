using System;
using System.Linq;
using Better.Plugins.PhotonEventBus.Runtime.Models;
using Better.Plugins.PhotonEventBus.Runtime.Models.Options;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using NetworkPlayer = Better.Plugins.PhotonEventBus.Runtime.Models.NetworkPlayer;

namespace Better.Plugins.PhotonEventBus.Runtime
{
    public static class PhotonDataConverter
    {
        public static object DeserializeNetworkPlayer(StreamBuffer inStream, short length)
        {
            var index = 0;
            var buffer = new byte[length];
            inStream.Read(buffer, 0, length);
            var vo = DeserializePlayer(buffer, ref index);

            return vo;
        }

        public static short SerializeNetworkPlayer(StreamBuffer outStream, object customObject)
        {
            var buffer = Array.Empty<byte>();
            Serialize((NetworkPlayer)customObject, ref buffer);
            outStream.Write(buffer, 0, buffer.Length);
            return (short)buffer.Length;
        }
        
        public static int GetPhotonOwnerID(this NetworkPlayer networkPlayer)
        {
            return networkPlayer.OwnerID;
        }

        public static int GetOwnerID(this Player networkPlayer)
        {
            return networkPlayer.ActorNumber;;
        }

        public static Hashtable Convert(this CustomProperties roomProperties)
        {
            var hashTable = new Hashtable();
            foreach (var property in roomProperties)
            {
                hashTable.Add(property.Key, property.Value);
            }

            return hashTable;
        }

        public static CustomProperties Convert(this Hashtable hashtable)
        {
            var roomProperties = new CustomProperties();
            foreach (var property in hashtable)
            {
                roomProperties.Add(property.Key, property.Value);
            }

            return roomProperties;
        }

        public static NetworkPlayer Convert(this Player player)
        {
            return new NetworkPlayer(player.ActorNumber, player.IsLocal, player.NickName,
                player.IsMasterClient,
                player.CustomProperties.Convert());
        }

        public static void Serialize(NetworkPlayer networkPlayer, ref byte[] bytes)
        {
            if (!networkPlayer.IsValid)
            {
                Serializer.Serialize(false, ref bytes);
                return;
            }

            Serializer.Serialize(networkPlayer.IsValid, ref bytes);
            Serializer.Serialize(networkPlayer.Nickname, ref bytes);
            Serializer.Serialize(networkPlayer.OwnerID, ref bytes);
            Serializer.Serialize(networkPlayer.IsHost, ref bytes);
        }

        public static NetworkPlayer DeserializePlayer(byte[] bytes, ref int offset)
        {
            var isValid = Serializer.DeserializeBool(bytes, ref offset);
            if (!isValid) return default;
            var nickname = Serializer.DeserializeString(bytes, ref offset);
            var ownerID = Serializer.DeserializeInt(bytes, ref offset);
            var isLocal = ownerID == PhotonNetwork.LocalPlayer.ActorNumber;
            var isHost = Serializer.DeserializeBool(bytes, ref offset);
            return new NetworkPlayer(ownerID, isLocal, nickname, isHost, new CustomProperties());
        }

        public static (RaiseEventOptions, SendOptions) Convert(this EventOptions eventOptions)
        {
            var r = eventOptions.ReceiverGroupOptions switch
            {
                ReceiverGroupsOptions.Others => ReceiverGroup.Others,
                ReceiverGroupsOptions.All => ReceiverGroup.All,
                ReceiverGroupsOptions.Host => ReceiverGroup.MasterClient,
                _ => throw new ArgumentOutOfRangeException()
            };

            var c = eventOptions.CashingOption switch
            {
                CashingEventOption.DoNotCache => EventCaching.DoNotCache,
                CashingEventOption.AddToRoomCache => EventCaching.AddToRoomCache,
                _ => throw new ArgumentOutOfRangeException()
            };

            var raiseEventOptions = new RaiseEventOptions
            {
                TargetActors = eventOptions.TargetActors.Length <= 0
                    ? null
                    : eventOptions.TargetActors.Select(x => x).ToArray(),
                InterestGroup = eventOptions.InterestGroup,
                Receivers = r,
                CachingOption = c
            };
            var sendOptions = new SendOptions
            {
                Channel = eventOptions.Channel,
                Reliability = eventOptions.Reliability
            };
            return (raiseEventOptions, sendOptions);
        }
    }
}