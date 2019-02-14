module Menu

open System.Windows.Forms
open Resources
open System

open Browser
open EnumerableExtensions   

let createAccelerator (menuItem: MenuItem) = 
    let key, alt, ctrl, shift = 
        match menuItem.Shortcut with
        | Shortcut.CtrlH -> (72, false, true, false)
        | Shortcut.CtrlR -> (82, false, true, false)
        | Shortcut.F3 -> (114, false, false, false)
        | Shortcut.F5 -> (116, false, false, false)
        | Shortcut.F7 -> (118, false, false, false)
        | Shortcut.F9 -> (120, false, false, false)
        | Shortcut.F11 -> (122, false, false, false)
        | Shortcut.F12-> (123, false, false, false)
        | _ -> (0, false, false, false)

    {
        MenuItem = menuItem
        Key = key
        Alt = alt
        Ctrl = ctrl
        Shift = shift
    }

let makeSeqFromEnumerator enumerator = 
    enumerator
    |> castEnumerator<MenuItem> 
    |> makeSeq 

let getSubMenuItems (menuItems: Menu.MenuItemCollection) = menuItems.GetEnumerator() |> makeSeqFromEnumerator

let createMenu (form: Form) (browser: Browser) browserForm =
    let menu = new MainMenu()
    let itemFile = new MenuItem(Resources.MenuFile)
    menu.MenuItems.Add itemFile |> ignore
    let itemRename = new MenuItem(Resources.MenuRename, EventHandler(fun s e -> ()), Shortcut.F2)
    itemFile.MenuItems.Add itemRename |> ignore
    itemFile.MenuItems.Add "-" |> ignore
//    let itemCopy = new MenuItem(Resources.MenuCopy, async (s, e) => await commander.FocusedView.Copy(commander.Other), Shortcut.F5);
//itemFile.MenuItems.Add(itemCopy);
    let itemMove = new MenuItem(Resources.MenuMove, EventHandler(fun s e -> ()), Shortcut.F6)
    itemFile.MenuItems.Add itemMove |> ignore
    let itemDelete = new MenuItem(Resources.MenuDelete, EventHandler(fun s e -> ()), Shortcut.Del)
    itemFile.MenuItems.Add itemDelete |> ignore
    itemFile.MenuItems.Add "-" |> ignore
//var itemCreateFolder = new MenuItem(Resources.MenuCreateFolder, 
//    async (s, e) => await commander.FocusedView.CreateFolder(), Shortcut.F7);
//itemFile.MenuItems.Add(itemCreateFolder);
//var itemProperties = new MenuItem(Resources.MenuProperties, (s, e) => { });
//itemFile.MenuItems.Add(itemProperties);
//itemFile.MenuItems.Add("-");
    let itemExit = new MenuItem(Resources.MenuExit, EventHandler(fun s e -> form.Close()), Shortcut.AltF4)
    itemFile.MenuItems.Add itemExit |> ignore

    let itemView = new MenuItem(Resources.MenuView)
    menu.MenuItems.Add itemView |> ignore

    let itemZoom = new MenuItem(Resources.MenuZoom)
    itemView.MenuItems.Add itemZoom |> ignore
    let itemZoom50 = new MenuItem("50%", EventHandler(fun s e -> browser.OnZoom(s :?> MenuItem, 50.0)))
    itemZoom50.RadioCheck <- true
    itemZoom.MenuItems.Add itemZoom50 |> ignore
    let itemZoom75 = new MenuItem("75%", EventHandler(fun s e -> browser.OnZoom(s :?> MenuItem, 75.0)))
    itemZoom75.RadioCheck <- true
    itemZoom.MenuItems.Add itemZoom75 |> ignore
    let itemZoom100 = new MenuItem("100%", EventHandler(fun s e -> browser.OnZoom(s :?> MenuItem, 100.0)))
    itemZoom100.Checked <- true
    itemZoom100.RadioCheck <- true
    itemZoom.MenuItems.Add itemZoom100 |> ignore
    let itemZoom150 = new MenuItem("150%", EventHandler(fun s e -> browser.OnZoom(s :?> MenuItem, 150.0)))
    itemZoom150.RadioCheck <- true
    itemZoom.MenuItems.Add itemZoom150 |> ignore
    let itemZoom200 = new MenuItem("200%", EventHandler(fun s e -> browser.OnZoom(s :?> MenuItem, 200.0)))
    itemZoom200.RadioCheck <- true
    itemZoom.MenuItems.Add itemZoom200 |> ignore
    let itemZoom250 = new MenuItem("250%", EventHandler(fun s e -> browser.OnZoom(s :?> MenuItem, 250.0)))
    itemZoom250.RadioCheck <- true
    itemZoom.MenuItems.Add itemZoom250 |> ignore
    let itemZoom300 = new MenuItem("300%", EventHandler(fun s e -> browser.OnZoom(s :?> MenuItem, 300.0)))
    itemZoom300.RadioCheck <- true
    itemZoom.MenuItems.Add itemZoom300 |> ignore
    let itemZoom350 = new MenuItem("350%", EventHandler(fun s e -> browser.OnZoom(s :?> MenuItem, 350.0)))
    itemZoom350.RadioCheck <- true
    itemZoom.MenuItems.Add itemZoom350 |> ignore
    let itemZoom400 = new MenuItem("400%", EventHandler(fun s e -> browser.OnZoom(s :?> MenuItem, 400.0)))
    itemZoom400.RadioCheck <- true
    itemZoom.MenuItems.Add itemZoom400 |> ignore

    let itemFullscreen = new MenuItem(Resources.MenuFullscreen, EventHandler(fun s e -> browserForm.ToFullScreen()), Shortcut.F11)
    itemView.MenuItems.Add itemFullscreen |> ignore
    itemView.MenuItems.Add "-" |> ignore
    let itemDevTools = new MenuItem(Resources.MenuDeveloperTools, EventHandler(fun s e -> browser.ShowDevTools ()), Shortcut.F12)
    itemView.MenuItems.Add itemDevTools |> ignore
    
    let getMenuItems (menuItems: Menu.MenuItemCollection) =
        getSubMenuItems menuItems
        |> Seq.collect (fun n -> getSubMenuItems n.MenuItems)

    let accelerators = 
        getMenuItems menu.MenuItems
        |> Seq.map createAccelerator
        |> Seq.filter (fun n -> n.Key <> 0)
        |> Seq.toArray
    
    browser.InitializeAccelerators accelerators
    
    let clearZoomItems () =
        getSubMenuItems itemZoom.MenuItems |> Seq.forall (fun n -> n.Checked <- false; true) |> ignore

    (menu, clearZoomItems)
