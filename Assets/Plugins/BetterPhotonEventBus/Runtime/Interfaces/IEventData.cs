namespace BetterPhotonEventBus.Interfaces
{
    /// <summary>
    /// Interface represents data types for Networking
    /// </summary>
    public interface IEventData
    {
        string ReceiverMethodName { get; }
    }
}