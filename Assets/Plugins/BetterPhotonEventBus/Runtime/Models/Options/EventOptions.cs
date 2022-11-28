namespace BetterPhotonEventBus.Models.Options
{
    public struct EventOptions
    {
        public static EventOptions HostOptions => new EventOptions(ReceiverGroupsOptions.Host);
        public static EventOptions OtherOptions => new EventOptions(ReceiverGroupsOptions.Others);
        public static EventOptions AllOptions => new EventOptions(ReceiverGroupsOptions.All);

        public ReceiverGroupsOptions ReceiverGroupOptions { get; }
        public byte Channel { get; }
        public byte InterestGroup { get; }
        public bool Reliability { get; }
        public CashingEventOption CashingOption { get; }
        public int[] TargetActors { get; }

        public EventOptions(ReceiverGroupsOptions receiverGroupOptions, CashingEventOption cashingOption,
            params int[] targetActors)
        {
            ReceiverGroupOptions = receiverGroupOptions;
            CashingOption = cashingOption;
            TargetActors = targetActors;
            Reliability = true;
            Channel = 0;
            InterestGroup = 0;
        }

        public EventOptions(ReceiverGroupsOptions receiverGroupOptions, byte channel, byte interestGroup, bool reliability,
            CashingEventOption cashingOption, params int[] targetActors)
        {
            ReceiverGroupOptions = receiverGroupOptions;
            Channel = channel;
            InterestGroup = interestGroup;
            Reliability = reliability;
            CashingOption = cashingOption;
            TargetActors = targetActors;
        }

        public EventOptions(ReceiverGroupsOptions receiverGroupOptions, bool reliability, params int[] targetActors)
        {
            ReceiverGroupOptions = receiverGroupOptions;
            Reliability = reliability;
            TargetActors = targetActors;
            CashingOption = CashingEventOption.DoNotCache;
            Channel = 0;
            InterestGroup = 0;
        }

        public EventOptions(ReceiverGroupsOptions receiverGroupOptions, params int[] targetActors)
        {
            ReceiverGroupOptions = receiverGroupOptions;
            TargetActors = targetActors;
            Reliability = true;
            CashingOption = CashingEventOption.DoNotCache;
            Channel = 0;
            InterestGroup = 0;
        }
    }
}