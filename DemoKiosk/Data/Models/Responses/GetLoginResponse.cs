namespace DemoKiosk.Data.Models.Responses
{
    internal class GetLoginResponse : BaseResponse
    {
        public bool isLoggedin { get; set; }
        public string? peerId { get; set; }
        public string? peerDn { get; set; }
    }
}