module Browser

open CefSharp
open CefSharp.WinForms
open System.Windows.Forms
open System

let mutable private commanderUrl = "serve://commander/"
let mutable private isAngularServing = false

let getCommanderUrl () = commanderUrl

let initialize (cmdLine: string[]) = 
    if cmdLine.Length > 0 && cmdLine.[0] = "-serve" then isAngularServing <- true
    if isAngularServing then commanderUrl <- "http://localhost:4200/"

[<NoComparison>]
[<NoEquality>]
type Host = {
    Control: Control
    GetFullScreenForm: unit->Form
    ExitFullScreen: unit->unit
}
    
[<NoComparison>]
type Accelerator = {
    MenuItem: MenuItem
    Key: int
    Alt: bool 
    Ctrl: bool 
    Shift: bool
}
    
type Browser (host, browser: ChromiumWebBrowser)  =
    let mutable accelerators: Accelerator[] = Array.empty

    member this.InitializeAccelerators value = accelerators <- value

    member this.ShowDevTools () = 
        browser.GetBrowser().ShowDevTools()

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

[<NoEquality>]
[<NoComparison>]
type BrowserForm = { ToFullScreen: unit->unit }
