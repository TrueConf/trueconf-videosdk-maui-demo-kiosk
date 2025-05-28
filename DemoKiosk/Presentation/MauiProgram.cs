#if WINDOWS
using DemoKiosk.Presentation;
using DemoKiosk.Presentation.Views.Windows;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Platform;
using Microsoft.UI;
using Microsoft.UI.Windowing;
#endif

namespace DemoKiosk
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
#if WINDOWS
            // window configuration through AppWindow
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddWindows(wndLifeCycleBuilder =>
                {
                    wndLifeCycleBuilder.OnWindowCreated(window =>
                    {
                        var wnd = window.GetWindow();
                        IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
                        var _appWindow = AppWindow.GetFromWindowId(myWndId);
                        if (wnd is MainWindow)
                        {
                            window.ExtendsContentIntoTitleBar = false;
                            _appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
                        }
                        else if (wnd is AlertWindow)
                        {
                            var display = DisplayArea.GetFromWindowId(myWndId, DisplayAreaFallback.Nearest);
                            _appWindow.MoveAndResize(new Windows.Graphics.RectInt32
                            (
                                (int)((display.WorkArea.Width - wnd.Width) / 2), 
                                (int)((display.WorkArea.Height - wnd.Height) / 2), 
                                (int)wnd.Width, 
                                (int)wnd.Height)
                            );
                            if (_appWindow.Presenter is OverlappedPresenter presenter)
                            {
                                presenter.IsResizable = false;
                                presenter.IsMaximizable = false;
                                presenter.IsMinimizable = false;
                            }
                        }
                    });
                });
            });
#endif
            return builder.Build();
        }
    }
}
