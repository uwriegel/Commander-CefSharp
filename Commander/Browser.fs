module Browser

open CefSharp
open CefSharp.WinForms

let mutable private commanderUrl = "serve://commander/"
let mutable private isAngularServing = false

let getCommanderUrl () = commanderUrl

let initialize (cmdLine: string[]) = 
    if cmdLine.Length > 0 && cmdLine.[0] = "-serve" then isAngularServing <- true
    if isAngularServing then commanderUrl <- "http://localhost:4200/"
    
type Browser = { ShowDevTools: unit->unit }

let createBrowser (browser: ChromiumWebBrowser) =
    let showDevTools () =
        browser.GetBrowser().ShowDevTools()
    
    {
        ShowDevTools = showDevTools
    }

type BrowerForm = { ToFullScreen: unit->unit }
