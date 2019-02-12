module Browser

open CefSharp
open CefSharp.WinForms
open System.Windows.Forms

let mutable private commanderUrl = "serve://commander/"
let mutable private isAngularServing = false

let getCommanderUrl () = commanderUrl

let initialize (cmdLine: string[]) = 
    if cmdLine.Length > 0 && cmdLine.[0] = "-serve" then isAngularServing <- true
    if isAngularServing then commanderUrl <- "http://localhost:4200/"
    
[<NoComparison>]
type Accelerator = {
    MenuItem: MenuItem;
    Key: int
    Alt: bool 
    Ctrl: bool 
    Shift: bool
}
    
type Browser(browser: ChromiumWebBrowser)  =
    let mutable accelerators: Accelerator[] = Array.empty

    member this.InitializeAccelerators value = accelerators <- value

    member this.ShowDevTools () = 
        browser.GetBrowser().ShowDevTools()

    interface IKeyboardHandler with
        member this.OnPreKeyEvent(chromiumWebBrowser: IWebBrowser, ibrowser: IBrowser, keytype: KeyType, windowsKeyCode: int, 
                                    nativeKeyCode: int, modifiers: CefEventFlags, isSystemKey: bool, isKeyboardShortcut& bool) =
            if keytype = KeyType.RawKeyDown then
                
                true
            else
                false
        member this.OnKeyEvent(chromiumWebBrowser: IWebBrowser, browser: IBrowser, keytype: KeyType, windowsKeyCode: int, 
                                nativeKeyCode: int, modifiers: CefEventFlags, isSystemKey: bool) =
            false

[<NoEquality>]
[<NoComparison>]
type BrowserForm = { ToFullScreen: unit->unit }
