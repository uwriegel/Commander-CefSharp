﻿module Menu

open System.Windows.Forms
open Resources
open System

open Browser
open System.Collections
open System.Collections.Generic
open System.Windows.Forms

let castEnumerator<'U> (enumerator: IEnumerator) = {
    new IEnumerator<'U> with
        member x.Current with get() = enumerator.Current :?> 'U
    interface IEnumerator with
        member x.Current with get() = enumerator.Current
        member x.MoveNext() = enumerator.MoveNext()
        member x.Reset() = enumerator.Reset()
    interface IDisposable with
        member x.Dispose() = ()
}      

let makeSeq enumerator = {
    new IEnumerable<'U> with
        member x.GetEnumerator() = enumerator
    interface IEnumerable with
        member x.GetEnumerator() = 
            (enumerator :> IEnumerator)
}

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
    let itemFullscreen = new MenuItem(Resources.MenuFullscreen, EventHandler(fun s e -> browserForm.ToFullScreen()), Shortcut.F11)
    itemView.MenuItems.Add itemFullscreen |> ignore
    itemView.MenuItems.Add "-" |> ignore
    let itemDevTools = new MenuItem(Resources.MenuDeveloperTools, EventHandler(fun s e -> browser.ShowDevTools ()), Shortcut.F12)
    itemView.MenuItems.Add itemDevTools |> ignore
    

    let schwein = menu.MenuItems.GetEnumerator() |> castEnumerator 
    let a = schwein.MoveNext()
    let b = schwein.Current
    let a1 = schwein.MoveNext()
    let b1 = schwein.Current
    let a2 = schwein.MoveNext()
    let b2 = schwein.Current
    let a3 = schwein.MoveNext()
    let b3 = schwein.Current
    let a4 = schwein.MoveNext()
    let b4 = schwein.Current
    let a5 = schwein.MoveNext()
    let b5 = schwein.Current
    let a6 = schwein.MoveNext()
    let b6 = schwein.Current
    let a7 = schwein.MoveNext()
    let b7 = schwein.Current
    let a8 = schwein.MoveNext()

    let makeSeqFromEnumerator enumerator = 
        enumerator
        |> castEnumerator<MenuItem> 
        |> makeSeq 

    let getSubMenuItems (menuItems: Menu.MenuItemCollection) = menuItems.GetEnumerator() |> makeSeqFromEnumerator

    let getMenuItems (menuItems: Menu.MenuItemCollection) =
        getSubMenuItems menuItems
        |> Seq.collect (fun n -> getSubMenuItems n.MenuItems)

    let accelerators = 
        getMenuItems menu.MenuItems
        |> Seq.map createAccelerator
        |> Seq.filter (fun n -> n.Key <> 0)
        |> Seq.toArray
    
    browser.InitializeAccelerators accelerators
    
    menu
