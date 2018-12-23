using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
#if DEBUG
                CachePath = cachePath,
                //LogSeverity = LogSeverity.Verbose,
                LogFile = Path.Combine(cachePath, "debug.log")
#endif
            };

            settings.CefCommandLineArgs.Add("enable-media-stream", "enable-media-stream");
            settings.CefCommandLineArgs.Add("disable-web-security", "disable-web-security");

            //settings.RegisterScheme(new CefCustomScheme
            //{
            //    SchemeName = CustomProtocolFactory.SchemeName,
            //    SchemeHandlerFactory = new CustomProtocolFactory()
            //});

            var res = CefSharp.Cef.Initialize(settings);
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            CefSharpSettings.ShutdownOnExit = true;
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
        }
    }
}
