using System;

namespace BetterPhotonEventBus.Models.EventTypes
{
    public class EventCodeType
    {
        public byte EventCode { get; }
        public Type Type { get; }
        public EventCodeType(Type type, byte eventCode)
        {
            Type = type;
            EventCode = eventCode;
        }

    }
}