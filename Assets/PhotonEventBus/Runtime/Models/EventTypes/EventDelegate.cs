using Better.PhotonEventBus.Runtime.Interfaces;

namespace Better.PhotonEventBus.Runtime.Models.EventTypes
{
    public delegate void EventDelegate<in T>(T data) where T : IEventData;
}