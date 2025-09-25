# Introduction

This guide describes how to work with the **TrueConf MAUI VideoSDK** for Desktop on Windows. It is a MAUI control (package) for creating enterprise applications with messenger and video communication features from TrueConf. It is essentially a wrapper around [TrueConf VideoSDK](https://trueconf.com/docs/videosdk/en/introduction/common), which implements its main functions.

To develop and use the application, you will need the [**TrueConf SDK** extension](https://trueconf.com/docs/server/en/admin/extensions/#sdk) in the license for your video conferencing server TrueConf Server or TrueConf Enterprise. For licensing details, please [contact TrueConf managers](https://trueconf.com/company/contacts.html).

## Requirements

System requirements for developing applications with TrueConf MAUI VideoSDK:

1. Windows 10+

1. .NET 8.0+

1. IDE MS Visual Studio 2022 with the .NET MAUI extension installed.

1. The purchased **TrueConf SDK** extension in the license on the video conferencing server that will be used to create test accounts and connect the application.

## How to start

To develop the application, follow these steps:

1. Install TrueConf VideoSDK version 5.0 or higher. You don't need to activate TrueConf VideoSDK after installation. You can download it from [this page](https://github.com/TrueConf/pyVideoSDK/blob/main/download.md#download-trueconf-videosdk-for-free).

1. After installing the SDK, restart Visual Studio so that the IDE can detect the new package.

1. Create a new .NET MAUI project in Visual Studio.

1. Right-click on your project in **Solution Explorer**.

1. Click on **Manage NuGet Packages**.

1. In the package manager that opens, select the **Browse** tab and select **Microsoft Visual Studio Offline Packages** from the **Package source** list in the upper right corner.

1. Find the **TrueConf.VideoSDK.MAUI** package in the search and click the **Install** button:

![Adding MAUI VideoSDK control](assets/add-sdk.png)

## How to use app created with MAUI SDK

In order for the application you have created to work on other PCs, you need to:

1. [Install TrueConf VideoSDK](https://github.com/TrueConf/pyVideoSDK/blob/main/download.md#download-trueconf-videosdk-for-free) on the target PC, using the same version that was used for development.

1. Activate the **TrueConf SDK** extension in the license on the video conferencing server to which the application will connect.

1. Run your application.

## Example

In this example we demonstrate how to get started with the TrueConf.VideoSDK.MAUI component: authorize and start a video call (peer-to-peer session). The application code contains comments, and some explanations are provided below.

The application itself consists of one main window. When launched, it connects to the video communication server and authorizes with the specified credentials. The only button, **Call**, is used to make a video call to the subscriber.

![Demo kiosk based on the TrueConf SDK](assets/demo.png)

### Authentication

Authorization is shown in <interface all='{Presentation}{Views}{Pages}{MainPage.xaml.cs}'/>. For demonstration purposes, the variables for connecting to the server are specified directly in the code. Creating your own authorization window or transferring parameters in another way is left to the discretion of the corporate application programmer:

```cs
// Server to connect to
private const string _trueConfServer = "<server.trueconf.com>";
// TrueConf ID of a user in the server
private const string _trueConfId = "<trueconf_id>";
// Password of the user
private const string _password = "<password>";
// TrueConfID of a user to call to
private const string _callTo = "<call to trueconf_id>";
```

The connection to the server is established immediately after launch using the function `_sdk.Methods.connectToServer(_trueConfServer)`.

After connecting to the server, the control changes its state, which is tracked by subscribing to the event `_sdk.OnAppStateChanged`. The function checks the new state and, if the connection to the server is successful, calls the function `_sdk.Methods.login(_trueConfId, _password)` for authorization:

```cs
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
```

### Call

The call is triggered when the button is pressed, which invokes the `_sdk.Methods.call(_callTo)` method. Note that the control state is checked beforehand using `_sdk.AppState`:

```cs
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
```

Incoming calls are answered automatically if the user is online. To answer a call, the `_sdk.Methods.accept()` function is called. Receiving a call is tracked by subscribing to the `_sdk.Events.On_inviteReceived` event. To handle the event, the `OnInviteReceived` function is called:

```cs
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
```

Read more in the [TrueConf MAUI VideoSDK documentation](https://trueconf.com/docs/maui-videosdk/en/introduction/common.html).