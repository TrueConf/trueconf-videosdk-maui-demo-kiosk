namespace DemoKiosk.Data.Models.Events
{
    internal class OnInviteReceivedEvent : BaseEvent
    {
        public string? peerId { get; set; }
        public string? peerDn { get; set; }
        public int type { get; set; }
        public string? confTitle { get; set; }
        public string? confId { get; set; }
    }
}