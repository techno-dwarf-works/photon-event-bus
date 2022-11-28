using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using RaiseEventBus.Runtime.Interfaces;
using RaiseEventBus.Runtime.Models;
using RaiseEventBus.Runtime.Models.Comparers;
using RaiseEventBus.Runtime.Models.EventTypes;
using RaiseEventBus.Runtime.Models.Options;

namespace RaiseEventBus.Runtime
{
    [Serializable]
    public class PhotonRaiseEventBus : IOnEventCallback
    {
        private Dictionary<EventCodeType, Delegate> _eventsDictionary;
        private HashSet<Type> _registeredEventTypes;

        public void Initialize()
        {
            _eventsDictionary = new Dictionary<EventCodeType, Delegate>(new EventCodeTypeComparer());
            _registeredEventTypes = new HashSet<Type>();

            PhotonPeer.RegisterType(typeof(NetworkPlayer), (byte)'N', SerializeNetworkPlayer, DeserializeNetworkPlayer);
            PhotonNetwork.AddCallbackTarget(this);
        }

        private object DeserializeNetworkPlayer(StreamBuffer inStream, short length)
        {
            var index = 0;
            var buffer = new byte[length];
            inStream.Read(buffer, 0, length);
            var vo = PhotonDataConverter.DeserializePlayer(buffer, ref index);

            return vo;
        }

        private short SerializeNetworkPlayer(StreamBuffer outStream, object customObject)
        {
            var buffer = Array.Empty<byte>();
            PhotonDataConverter.Serialize((NetworkPlayer)customObject, ref buffer);
            outStream.Write(buffer, 0, buffer.Length);
            return (short)buffer.Length;
        }

        public void Deconstruct()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void RegisterEventType<T>()
        {
            var type = typeof(T);
            if (_registeredEventTypes.Contains(type)) return;
            _registeredEventTypes.Add(type);
        }

        /// <summary>
        /// Method to register custom event data. Bytes are reserved by Photon 'W'(87), 'V'(86), 'Q'(81), 'P'(80).
        /// </summary>
        /// <param name="code"></param>
        /// <typeparam name="T"></typeparam>
        public void RegisterEventType<T>(byte code) where T : IEventData
        {
            Serializer.RegisterCustomType<T>(code);
            RegisterEventType<T>();
        }

        public void Subscribe<T>(EventDelegate<T> action, byte customEventCode = 0) where T : IEventData
        {
            var type = typeof(T);
            var eventCodeType = new EventCodeType(type, customEventCode);
            if (_eventsDictionary.ContainsKey(eventCodeType))
            {
                _eventsDictionary[eventCodeType] = Delegate.Combine(_eventsDictionary[eventCodeType], action);
            }
            else
            {
                _eventsDictionary.Add(eventCodeType, action);
            }
        }

        public void Unsubscribe<T>(EventDelegate<T> action, byte customEventCode = 0) where T : IEventData
        {
            var type = typeof(T);
            var eventCodeType = new EventCodeType(type, customEventCode);
            if (_eventsDictionary.ContainsKey(eventCodeType))
            {
                _eventsDictionary[eventCodeType] = Delegate.Remove(_eventsDictionary[eventCodeType], action);
            }
        }

        private void Invoke(IEventData eventData, byte customEventCode)
        {
            var type = eventData.GetType();
            var eventCodeType = new EventCodeType(type, customEventCode);
            if (!_eventsDictionary.TryGetValue(eventCodeType, out var value)) return;
            if (value == null) return;
            if (eventData.ReceiverMethodName.Equals(string.Empty))
            {
                value.DynamicInvoke(eventData);
            }
            else
            {
                foreach (var callMethod in value.GetInvocationList()
                             .Where(x => x != null && x.Method.Name.Equals(eventData.ReceiverMethodName)))
                {
                    callMethod.DynamicInvoke(eventData);
                }
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code > 0) return;
            var data = photonEvent.CustomData;
            if (!(data is IEventData eventData)) return;
            if (_registeredEventTypes.Contains(eventData.GetType()))
            {
                Invoke(eventData, photonEvent.Code);
            }
        }

        public void SendEvent<T>(T eventData, EventOptions eventOptions, byte customEventCode = 0) where T : IEventData
        {
            var (raiseEventOptions, sendOptions) = eventOptions.Convert();
            PhotonNetwork.RaiseEvent(customEventCode, eventData, raiseEventOptions, sendOptions);
        }
    }
}