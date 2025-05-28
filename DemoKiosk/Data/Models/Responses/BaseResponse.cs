namespace DemoKiosk.Data.Models.Responses
{
    internal class BaseResponse
    {
        public string? method { get; set; }
        public string? requestId { get; set; }
        public bool result { get; set; }
    }
}