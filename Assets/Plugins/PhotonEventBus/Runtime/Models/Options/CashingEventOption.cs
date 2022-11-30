namespace Better.Plugins.PhotonEventBus.Runtime.Models.Options
{
    public enum CashingEventOption
    {
        DoNotCache = 0,

        /// <summary>Adds an event to the room's cache</summary>
        AddToRoomCache = 4,
    }
}