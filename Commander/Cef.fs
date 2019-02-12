module Cef

open System
open System.IO
open System.Reflection
open System.Threading

open CefSharp
open CefSharp.WinForms

let initialize () =
    let cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefControl")
    let settings = 
        new CefSettings(
            BrowserSubprocessPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "CefSharp.BrowserSubprocess.exe"),
            Locale = Thread.CurrentThread.CurrentCulture.Name,
            CachePath = cachePath,
            //LogSeverity = LogSeverity.Verbose
            LogFile = Path.Combine(cachePath, "cef.log")
        ) 

    settings.CefCommandLineArgs.Add("enable-media-stream", "enable-media-stream")
    settings.CefCommandLineArgs.Add("disable-web-security", "disable-web-security")

    CefSharp.Cef.Initialize settings |> ignore
    CefSharpSettings.LegacyJavascriptBindingEnabled <- true
    CefSharpSettings.ShutdownOnExit <- true
    CefSharpSettings.SubprocessExitIfParentProcessClosed <- true
