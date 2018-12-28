using System;
using System.IO;
using System.Reflection;
using System.Threading;

using CefSharp;
using CefSharp.WinForms;

namespace Commander
{
    static class Cef
    {
        public static void Initialize() { }

        static Cef()
        {
            var cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefControl");
            var settings = new CefSettings
            {
                BrowserSubprocessPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "CefSharp.BrowserSubprocess.exe"),
                Locale = Thread.CurrentThread.CurrentCulture.Name,
                CachePath = cachePath,
#if DEBUG
                //LogSeverity = LogSeverity.Verbose,
                LogFile = Path.Combine(cachePath, "debug.log")
#endif
            };

            settings.CefCommandLineArgs.Add("enable-media-stream", "enable-media-stream");
            settings.CefCommandLineArgs.Add("disable-web-security", "disable-web-security");

            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = CustomProtocolFactory.SchemeName,
                SchemeHandlerFactory = new CustomProtocolFactory()
            });

            var res = CefSharp.Cef.Initialize(settings);
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            CefSharpSettings.ShutdownOnExit = true;
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
        }
    }
}
