namespace DemoKiosk.Data.Models.Events
{
    internal class LoginEvent : BaseEvent
    {
        public int result { get; set; }
        public string? peerId { get; set; }
        public string? peerDn { get; set; }
    }
}
