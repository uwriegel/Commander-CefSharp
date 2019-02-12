module Menu

open System.Windows.Forms
open Resources
open System

let createMenu (form: Form) =
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
    itemFile.MenuItems.Add(itemExit) |> ignore
    menu
