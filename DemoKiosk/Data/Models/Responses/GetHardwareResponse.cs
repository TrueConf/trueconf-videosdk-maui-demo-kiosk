namespace DemoKiosk.Data.Models.Responses
{
    internal class GetHardwareResponse : BaseResponse
    {
        public HardwareElement[]? audioCapturers { get; set; }
        public string? currentAudioCapturerName { get; set; }
        public string? currentAudioCapturerDescription { get; set; }
        public int currentAudioCapturerType { get; set; }
        public HardwareElement[]? audioRenderers { get; set; }
        public string? currentAudioRendererName { get; set; }
        public string? currentAudioRendererDescription { get; set; }
        public int currentAudioRendererType { get; set; }
        public HardwareElement[]? videoCapturers { get; set; }
        public string? currentVideoCapturerName { get; set; }
        public string? currentVideoCapturerDescription { get; set; }
        public int currentVideoCapturerType { get; set; }
        public string? defaultContentSource { get; set; }
        public DSCaptureElement[]? DSCaptureList { get; set; }
        public string[]? comPorts { get; set; }
    }

    public class HardwareElement
    {
        public string? name { get; set; }
        public string? description { get; set; }
        public int type { get; set; }
    }

    public class DSCaptureElement
    {
        public string? name { get; set; }
        public long id { get; set; }
    }
}