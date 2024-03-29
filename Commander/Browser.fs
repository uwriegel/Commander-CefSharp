﻿module Browser

open CefSharp
open CefSharp.WinForms
open System.Windows.Forms
open System
open Commander

[<NoComparison>]
[<NoEquality>]
type Host = {
    Control: Control
    GetFullScreenForm: unit->Form
    ExitFullScreen: unit->unit
    ClearZoomItems: unit->unit
    MainWindow: nativeint
}
    
[<NoComparison>]
type Accelerator = {
    MenuItem: MenuItem
    Key: int
    Alt: bool 
    Ctrl: bool 
    Shift: bool
}
    
type Browser (host, browser: ChromiumWebBrowser) as this =
    let mutable accelerators: Accelerator[] = Array.empty

    let executeScript clss (method: string) (param: 'T option) = 
        let json = 
            match param with
            | Some value ->
                Json.serialize value
            | None -> ""
        browser.EvaluateScriptAsync(clss + "." + method + "(" + json + ")") |> Async.AwaitTask

    let executeScriptWithOptions clss (method: string) (param: 'T) = 
        let json = Json.serializeWithOptions param
        browser.EvaluateScriptAsync(clss + "." + method + "(" + json + ")") |> Async.AwaitTask

    let executeScriptWithParams clss (method: string) (parameters: obj[]) =
        browser.EvaluateScriptAsync(clss + "." + method, parameters) |> Async.AwaitTask

    let onMouseWheel (delta: double) = 
        this.ZoolLevel <- this.ZoolLevel + if delta > 0.0 then 10.0 else -10.0
        host.ClearZoomItems ()
    
    let leftView = CommanderView({ 
        GetRecentPath = (fun () -> Resources.Settings.Default.LeftRecentPath)
        SetRecentPath = (fun path -> Resources.Settings.Default.LeftRecentPath <- path)
        ExecuteScript = executeScript "commanderViewLeft"
        ExecuteCommanderScript = executeScript "commander"
        ExecuteCommanderScriptWithOptions = executeScriptWithOptions "commander"
        ExecuteScriptWithParams = executeScriptWithParams "commanderViewLeft"
        MainWindow = host.MainWindow
        Dispatcher = browser
    })
    let rightView = CommanderView({ 
        GetRecentPath = (fun () -> Resources.Settings.Default.RightRecentPath)
        SetRecentPath = (fun path -> Resources.Settings.Default.RightRecentPath <- path)
        ExecuteScript = executeScript "commanderViewRight"
        ExecuteCommanderScript = executeScript "commander"
        ExecuteCommanderScriptWithOptions = executeScriptWithOptions "commander"
        ExecuteScriptWithParams = executeScriptWithParams "commanderViewRight"
        MainWindow = host.MainWindow
        Dispatcher = browser
    })
    let commander = CommanderControl(leftView, rightView)
    let viewer = Viewer(host.Control)

    do 
        leftView.Other <- rightView
        rightView.Other <- leftView
        browser.RegisterJsObject("CommanderLeft", leftView, BindingOptions(CamelCaseJavascriptNames = true))        
        browser.RegisterJsObject("CommanderRight", rightView, BindingOptions(CamelCaseJavascriptNames = true))      
        browser.RegisterJsObject("CommanderControl", commander, BindingOptions(CamelCaseJavascriptNames = true))
        browser.RegisterJsObject("Viewer", viewer, BindingOptions(CamelCaseJavascriptNames = true))      
        browser.RegisterJsObject("MouseWheelZoomControl", MouseWheelZoomControl(onMouseWheel), 
                                    BindingOptions(CamelCaseJavascriptNames = true))

    let mutable zoomLevel = 0.0

    member this.ZoolLevel 
        with get() = zoomLevel
        and set value = 
            let newValue = 
                if value > 400.0 then 400.0
                elif value < 50.0 then 50.0
                else value
            zoomLevel <- newValue
            browser.SetZoomLevel(Math.Log(zoomLevel / 100.0) / Math.Log 1.2)

    member this.InitializeAccelerators value = accelerators <- value

    member this.Copy () = commander.Copy ()
    member this.CreateFolder () = commander.CreateFolder ()
    member this.AdaptPath () = commander.AdaptPath ()
    member this.Refresh () = commander.Refresh ()
    member this.SetTheme(theme: string) =
        browser.EvaluateScriptAsync ("themes.theme = '" + theme + "'") |> ignore

    member this.ShowDevTools () = 
        browser.GetBrowser().ShowDevTools()

    member this.OnZoom(zoomLevel) = this.ZoolLevel <- zoomLevel

    member this.OnViewer (activate: bool) =
        viewer.Show activate;
        browser.EvaluateScriptAsync ("commander.setViewer", activate) |> ignore

    member this.ShowHidden (showHidden: bool) =
        Globals.showHidden <- showHidden
        leftView.Refresh ()
        rightView.Refresh ()

    interface IKeyboardHandler with
        member this.OnPreKeyEvent(chromiumWebBrowser: IWebBrowser, ibrowser: IBrowser, keytype: KeyType, windowsKeyCode: int, 
                                    nativeKeyCode: int, modifiers: CefEventFlags, isSystemKey: bool, isKeyboardShortcut& bool) =
            let findAccelerator accelerator =
                accelerator.Key = windowsKeyCode 
                && (if modifiers.HasFlag(CefEventFlags.AltDown) then accelerator.Alt else not accelerator.Alt)
                && (if modifiers.HasFlag(CefEventFlags.ShiftDown) then accelerator.Shift else not accelerator.Shift)
                && (if modifiers.HasFlag(CefEventFlags.ControlDown) then accelerator.Ctrl  else not accelerator.Ctrl)
            
            if keytype = KeyType.RawKeyDown then
                match accelerators |> Seq.tryFind findAccelerator with
                | Some value -> 
                    host.Control.Invoke(Action(fun () -> value.MenuItem.PerformClick())) |> ignore
                    true
                | None -> 
                    let fullScreenForm = host.GetFullScreenForm ()
                    if fullScreenForm <> null && windowsKeyCode = LanguagePrimitives.EnumToValue Keys.Escape then 
                        host.Control.Invoke(Action(host.ExitFullScreen)) |> ignore
                        true
                    else
                        false
            else
                false
        member this.OnKeyEvent(chromiumWebBrowser: IWebBrowser, browser: IBrowser, keytype: KeyType, windowsKeyCode: int, 
                                nativeKeyCode: int, modifiers: CefEventFlags, isSystemKey: bool) =
            false

    interface ILoadHandler with
        member this.OnFrameLoadStart(browserControl: IWebBrowser, frameLoadStartArgs: FrameLoadStartEventArgs) = ()
        member this.OnFrameLoadEnd(browserControl: IWebBrowser, frameLoadEndArgs: FrameLoadEndEventArgs) = 
            if frameLoadEndArgs.Frame.IsMain && frameLoadEndArgs.Frame.Url = Globals.getCommanderUrl () then
                browser.EvaluateScriptAsync("themes.theme = '" + Resources.Settings.Default.Theme + "'") |> ignore
                browser.EvaluateScriptAsync(@"document.addEventListener('mousewheel', e => {
    if (e.ctrlKey) {
        MouseWheelZoomControl.onMouseWheel(e.wheelDelta)
        e.stopPropagation()
        e.preventDefault()
    }
}, true)"           
                ) |> ignore
                host.Control.BeginInvoke(Action(fun () -> browser.Focus ()|> ignore)) |> ignore

        member this.OnLoadError(browserControl: IWebBrowser, loadErrorArgs: LoadErrorEventArgs) = ()
        member this.OnLoadingStateChange(browserControl: IWebBrowser, loadingStateChangedArgs: LoadingStateChangedEventArgs) = ()
    
    interface IContextMenuHandler with
        member this.OnBeforeContextMenu(chromiumWebBrowser: IWebBrowser, browser: IBrowser, frame: IFrame, parameters: IContextMenuParams, model: IMenuModel) =
            model.Clear () |> ignore
        member this.OnContextMenuCommand(chromiumWebBrowser: IWebBrowser, browser: IBrowser, frame: IFrame, 
                                            parameters: IContextMenuParams, commandId: CefMenuCommand, eventFlags: CefEventFlags) = false
        member this.OnContextMenuDismissed(chromiumWebBrowser: IWebBrowser, browser: IBrowser, frame: IFrame) = ()
        member this.RunContextMenu(chromiumWebBrowser: IWebBrowser, browser: IBrowser, frame: IFrame, 
                                        parameters: IContextMenuParams, model: IMenuModel, callback: IRunContextMenuCallback) = false

[<NoEquality>]
[<NoComparison>]
type BrowserForm = { ToFullScreen: unit->unit }
