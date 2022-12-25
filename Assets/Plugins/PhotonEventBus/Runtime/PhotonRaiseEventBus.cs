using System;
using System.Collections.Generic;
using System.Linq;
using Better.Plugins.PhotonEventBus.Runtime.Interfaces;
using Better.Plugins.PhotonEventBus.Runtime.Models.Comparers;
using Better.Plugins.PhotonEventBus.Runtime.Models.EventTypes;
using Better.Plugins.PhotonEventBus.Runtime.Models.Options;
using ExitGames.Client.Photon;
using MSLIMA.Serializer;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using NetworkPlayer = Better.Plugins.PhotonEventBus.Runtime.Models.NetworkPlayer;

namespace Better.Plugins.PhotonEventBus.Runtime
{
    [Serializable]
    public class PhotonRaiseEventBus : IOnEventCallback
    {
        private Dictionary<EventCodeType, Delegate> _eventsDictionary;
        private HashSet<Type> _registeredEventTypes;
        private HashSet<byte> _registeredByteCodes;

        public void Initialize()
        {
            _eventsDictionary = new Dictionary<EventCodeType, Delegate>(new EventCodeTypeComparer());
            _registeredEventTypes = new HashSet<Type>();

            _registeredByteCodes = new HashSet<byte>()
            {
                (byte)'N', (byte)'W', (byte)'V', (byte)'Q', (byte)'P'
            };

            PhotonPeer.RegisterType(typeof(NetworkPlayer), (byte)'N', PhotonDataConverter.SerializeNetworkPlayer,
                PhotonDataConverter.DeserializeNetworkPlayer);
            PhotonNetwork.AddCallbackTarget(this);
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
            if (_registeredByteCodes.Contains(code))
            {
                Debug.LogError($"Key code: {code} already registered.");
                return;
            }
            
            var type = typeof(T);
            if (_registeredEventTypes.Contains(type))
            {
                Debug.LogError($"Type: {type} already registered.");
                return;
            }

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