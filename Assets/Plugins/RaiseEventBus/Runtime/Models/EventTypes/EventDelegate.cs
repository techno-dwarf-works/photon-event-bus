using RaiseEventBus.Runtime.Interfaces;

namespace RaiseEventBus.Runtime.Models.EventTypes
{
    public delegate void EventDelegate<in T>(T data) where T : IEventData;
}