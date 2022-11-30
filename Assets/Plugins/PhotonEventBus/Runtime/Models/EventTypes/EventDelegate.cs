using Better.Plugins.PhotonEventBus.Runtime.Interfaces;

namespace Better.Plugins.PhotonEventBus.Runtime.Models.EventTypes
{
    public delegate void EventDelegate<in T>(T data) where T : IEventData;
}