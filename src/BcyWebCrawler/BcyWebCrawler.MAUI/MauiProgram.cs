// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Maui.LifecycleEvents;
#if MACCATALYST
using CoreGraphics;
using UIKit;
#endif

namespace BcyWebCrawler.MAUI
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
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddWindows(wndLifeCycleBuilder =>
                {
                    wndLifeCycleBuilder.OnWindowCreated(window =>
                    {
                        IntPtr nativeWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        Microsoft.UI.WindowId win32WindowsId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(nativeWindowHandle);
                        Microsoft.UI.Windowing.AppWindow winuiAppWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(win32WindowsId);
                        if (winuiAppWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter p)
                        {
                            p.Maximize();
                            //p.IsResizable = false;
                            //p.IsMaximizable = false;
                            //p.IsMinimizable = false;
                        }
                    });
                });
            });
#endif
#if MACCATALYST
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddiOS(iosLifecycleBuilder =>
                {
                    iosLifecycleBuilder.OnActivated(del =>
                    {
                        //var vKeyWindow = e.KeyWindow;
                        var vKeyWindow = del.Windows.FirstOrDefault();
                        if (vKeyWindow is null)
                            return;

                        //var vTitleBar = vKeyWindow.WindowScene?.Titlebar;
                        //if (vTitleBar is null)
                        //    return;

                        //vTitleBar.TitleVisibility = UITitlebarTitleVisibility.Hidden;
                        //vTitleBar.Toolbar = null;

                        double nWidth = 1280;
                        double nHeight = 720;

                        var vScreen = vKeyWindow.Screen;
                        var vCGRect = vScreen.Bounds;

                        if (nWidth > vCGRect.Width)
                            nWidth = vCGRect.Width.Value;

                        if (nHeight > vCGRect.Height)
                            nHeight = vCGRect.Height.Value;

                        var windowScene = vKeyWindow.WindowScene;
                        if (windowScene is null)
                        {
                            return;
                        }
                        var sizeRestrictions = windowScene.SizeRestrictions;
                        if (sizeRestrictions is null)
                        {
                            return;
                        }

                        sizeRestrictions.MinimumSize = new CGSize(nWidth, nHeight);
                        sizeRestrictions.MaximumSize = new CGSize(vCGRect.Width, vCGRect.Height);
                    });
                });
            });
#endif

            return builder.Build();
        }
    }
}
