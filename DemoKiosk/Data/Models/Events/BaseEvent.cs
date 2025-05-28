using Newtonsoft.Json;

namespace DemoKiosk.Data.Models.Events
{
    internal class BaseEvent
    {
        [JsonProperty("event")] // there is a keyword in C# called event, we can't name variables this way
        public string? eventType { get; set; }
        public string? method { get; set; }
    }
}
