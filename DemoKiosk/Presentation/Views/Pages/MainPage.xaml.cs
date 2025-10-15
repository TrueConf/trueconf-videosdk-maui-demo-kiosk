using DemoKiosk.Data.Models.Events;
using DemoKiosk.Data.Models.Responses;
using Newtonsoft.Json.Linq;
using TrueConfVideoSDK.API;
using TrueConfVideoSDK.Exceptions;

namespace DemoKiosk.Presentation.Views.Pages;
public partial class MainPage : ContentPage
{
    private VideoSDK _sdk;
    private Action<BaseResponse> OnResponse = delegate { };

    // Server to connect to
    private const string _trueConfServer = "<server.trueconf.com>";
    // TrueConf ID of a user in the server
    private const string _trueConfId = "<trueconf_id>";
    // Password of the user
    private const string _password = "<password>";
    // TrueConfID of a user to call to
    private const string _callTo = "<call to trueconf_id>";

    public MainPage()
    {
        InitializeComponent();
        this.Loaded += (sender, e) =>
        {
            // Subscribing to an event that indicates start of the embedded VideoSDK application
            this.VideoSDKControl.OnAfterStart += VideoSDK_OnAfterStart;
            // Shutting down VideoSDK when this window is getting destroyed
            this.Window.Destroying += (sender, e) =>
            {
                this.VideoSDKControl.Shutdown();
            };
        };
    }

    /// <summary>
    /// Notification that indicates of the final initialization
    /// </summary>
    private void VideoSDK_OnAfterStart()
    {
        try
        {
            _sdk = this.VideoSDKControl.SDK;

            // Subscribing to SDK notifications
            // Response to all methods
            _sdk.Methods.OnMethodResponse += OnMethodResponse;
            // Notification about an incoming video call or about an invitation to a groupConference
            _sdk.Events.On_inviteReceived += OnInviteReceived;
            // Notification received after authorization on the server
            _sdk.Events.On_login += OnLogin;
            // Notification indicating that the application state has changed
            _sdk.OnAppStateChanged += OnAppStateChanged;

            // Resetting auth and connecting to a specified constant server
            // Log out from the server.
            _sdk.Methods.logout();
            // Connect to the specified server
            _sdk.Methods.connectToServer(_trueConfServer);

            // Subscribing to a response from getHardware
            OnResponse += GetHardwareInitialHandler;
            // Get the list of devices
            _sdk.Methods.getHardware();
        }
        catch (SessionOperationException ex)
        {
            HandleException(ex);
        }
    }

    /// <summary>
    /// Notification about an incoming video call or about an invitation to a groupConference
    /// </summary>
    private void OnInviteReceived(object? sender, string jsonStr)
    {
        // If we are in call or we are not doing anything we reject
        if (_sdk.AppState <= 3 || _sdk.AppState == 5)
        {
            // Reject an incoming call or invitation to a group conference
            _sdk.Methods.reject();
        }

        // Accept an incoming call or invitation to a group conference
        _sdk.Methods.accept(); 
    }

    /// <summary>
    /// Notification received after authorization on the server
    /// </summary>
    private void OnLogin(object? sender, string jsonStr)
    {
        JObject obj = JObject.Parse(jsonStr); // Parse the response from the event
        if (obj == null) return;

        try
        {
            LoginEvent loginEvent = obj.ToObject<LoginEvent>(); // Cast to an object that we can use
            if (loginEvent.result == 8) // If we are not allowed to log in with our application type
            {
                // Show an alert to the user
                var alert = new AlertWindow("Unauthorized", "You are not authorized to log in with this application type.\nThe application will now close...");
                Application.Current?.OpenWindow(alert);
                // When the alert is closed, we close the DemoKiosk application
                alert.Destroying += (sender, e) =>
                {
                    Application.Current?.CloseWindow(this.Window); 
                };
            }
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    /// <summary>
    /// A method that receives all of the responses from calls to VideoSDK
    /// </summary>
    private void OnMethodResponse(object? sender, string e)
    { 
        JObject obj = JObject.Parse(e); // Parse the response from the method
        if (obj == null) return; 

        try
        {
            // Read the method type
            string methodValue = obj["method"].Value<string>();

            // Deserialize it
            BaseResponse? response = null;
            switch (methodValue)
            {
                case "getHardware":
                    response = obj.ToObject<GetHardwareResponse>();
                    break;
                default:
                    break;
            }

            // Call an Action, if method result was positive (meaning it worked without errors)
            if (response != null && response.result != false)
                OnResponse(response);
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    /// <summary>
    /// Notification indicating that the application state has changed
    /// </summary>
    private void OnAppStateChanged(object? sender, AppStateChangedEventArgs e)
    {
        switch (e.NewState)
        {
            case 0: // No connection to server
            case 1: // Trying to connect to server
                break;
            case 2: // Connected, need to log in
                _sdk.Methods.login(_trueConfId, _password);
                break;
            case 4: // In waiting (receiving an invite or calling someone)
            case 5: // In conference
                ActionBtn.Text = "Reject";
                break;
            default:
                ActionBtn.Text = "Call";
                break;
        }
    }

    /// <summary>
    /// Receiver of click events of the main action button
    /// </summary>
    private async void ActionBtn_Clicked(object sender, EventArgs e)
    {
        if (_sdk == null)
        {
            // We show an alert this way, because MAUI doesn't allow internal alerts to be configured topmost of all child windows
            var alert = new AlertWindow("Error", "Application must be connected to TrueConf server and login to call.");
            this.Window.AddLogicalChild(alert);
            Application.Current?.OpenWindow(alert);
            return;
        }

        switch (_sdk.AppState)
        {
            case 0: // No connection to server
            case 1: // Trying to connect to server
            case 2: // Connected, need to log in
                var alert = new AlertWindow("Error", "Application must be connected to TrueConf server and login to call.");
                this.Window.AddLogicalChild(alert);
                Application.Current?.OpenWindow(alert);
                break;
            case 3: // Connected and logged in
                _sdk.Methods.call(_callTo);
                break;
            case 4: // In waiting (receiving an invite or calling someone)
                _sdk.Methods.hangUp();
                break;
            case 5: // In conference
                _sdk.Methods.hangUp();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// A custom handler of a getHardware method response
    /// </summary>
    /// <param name="resp">Response model</param>
    private void GetHardwareInitialHandler(BaseResponse resp)
    {
        if (resp is not GetHardwareResponse hardwareResp)
            return;

        try
        {
            // Get the first microphone from the audioCapturer list
            HardwareElement audioCapturer = hardwareResp.audioCapturers.FirstOrDefault();
            // Get the first speaker from the audioRenderer list
            HardwareElement audioRenderer = hardwareResp.audioRenderers.FirstOrDefault();
            // Get the first camera from the videoCapturer list
            HardwareElement videoCapturer = hardwareResp.videoCapturers.FirstOrDefault();

            // If any of the devices is null we simply exit
            if (audioCapturer == null || audioRenderer == null || videoCapturer == null)
                return;

            // Set the previously found devices
            // Set the first microphone from the audioCapturer list
            _sdk.Methods.setAudioCapturer(audioCapturer.name);
            // Set the first speaker from the audioRenderer list
            _sdk.Methods.setAudioRenderer(audioRenderer.name);
            // Set the first camera from the videoCapturer list
            _sdk.Methods.setVideoCapturer(videoCapturer.name);
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }

        // Unsubscribe from the OnResponse Action
        OnResponse -= GetHardwareInitialHandler;
    }

    /// <summary>
    /// Handles thrown exceptions
    /// </summary>
    private void HandleException(Exception ex)
    {
        Dispatcher.Dispatch(() =>
        {
            // We show an alert this way, because MAUI doesn't allow internal alerts to be configured topmost of all child windows
            var alert = new AlertWindow("Exception", ex.Message);
            Application.Current?.OpenWindow(alert);
        });
    }
}