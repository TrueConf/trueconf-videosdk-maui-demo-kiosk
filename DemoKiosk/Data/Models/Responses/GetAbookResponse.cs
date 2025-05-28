namespace DemoKiosk.Data.Models.Responses
{
    internal class GetAbookResponse : BaseResponse
    {
        public AddressBook[]? abook { get; set; }
    }

    public class AddressBook
    {
        public bool isEditable { get; set; }
        public string? peerId { get; set; }
        public string? peerDn { get; set; }
        public int status { get; set; }
        public int extStatus { get; set; }
        public long lastOnlineTime { get; set; }
        public string? additionalStatus { get; set; }
    }
}