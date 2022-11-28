using BetterPhotonEventBus.Interfaces;

namespace BetterPhotonEventBus.Models.EventTypes
{
    public delegate void EventDelegate<in T>(T data) where T : IEventData;
}