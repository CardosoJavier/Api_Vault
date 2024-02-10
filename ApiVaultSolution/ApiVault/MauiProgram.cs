/*
 *  Provides the entry point for the application. 
 *  
 *  It enables the application to start from a single location 
 *  and is used to configure fonts, services, and third-party libraries.
 */

#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using WinRT.Interop;
#endif

using Microsoft.Maui.LifecycleEvents;
using Colors = Microsoft.Maui.Graphics.Colors;
using Microsoft.Maui.Platform;

namespace ApiVault
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder().UseMauiApp<App>();

            // Load application styles
            setAppFonts(builder);
            setTitleBarButtonsColor(builder);

            return builder.Build();
        }

        /*
         * Loads the fonts from Resources/Fonts
         * @param builder
         */
        private static void setAppFonts(MauiAppBuilder builder)
        {
            // Font configuration
            builder.ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
        }

        /*
         * Loads the fonts from Resources/Fonts
         * @param builder
         */
        private static void setTitleBarButtonsColor(MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events =>
            {
#if WINDOWS
                events.AddWindows(windowsLifecycleBuilder =>
                {
                    windowsLifecycleBuilder.OnWindowCreated(window =>
                    {
                        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);
                        var titleBar = appWindow.TitleBar;

                        titleBar.ButtonForegroundColor = Colors.White.ToWindowsColor();
                        titleBar.InactiveForegroundColor = Colors.White.ToWindowsColor();
                    });
                });
#endif
            });
        }
    }
}

