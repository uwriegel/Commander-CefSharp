module Menu

open System.Windows.Forms
open Resources
open System

let createMenu (form: Form) =
    let menu = new MainMenu()
    let itemFile = new MenuItem(Resources.MenuFile)
    menu.MenuItems.Add(itemFile) |> ignore
    let itemRename = new MenuItem(Resources.MenuRename, EventHandler(fun s e -> ()), Shortcut.F2)
    itemFile.MenuItems.Add(itemRename) |> ignore
    itemFile.MenuItems.Add("-") |> ignore
//    let itemCopy = new MenuItem(Resources.MenuCopy, async (s, e) => await commander.FocusedView.Copy(commander.Other), Shortcut.F5);
//itemFile.MenuItems.Add(itemCopy);
//var itemMove = new MenuItem(Resources.MenuMove, (s, e) => { }, Shortcut.F6);
//itemFile.MenuItems.Add(itemMove);
//var itemDelete = new MenuItem(Resources.MenuDelete, (s, e) => { }, Shortcut.Del);
//itemFile.MenuItems.Add(itemDelete);
//itemFile.MenuItems.Add("-");
//var itemCreateFolder = new MenuItem(Resources.MenuCreateFolder, 
//    async (s, e) => await commander.FocusedView.CreateFolder(), Shortcut.F7);
//itemFile.MenuItems.Add(itemCreateFolder);
//var itemProperties = new MenuItem(Resources.MenuProperties, (s, e) => { });
//itemFile.MenuItems.Add(itemProperties);
//itemFile.MenuItems.Add("-");
    let itemExit = new MenuItem(Resources.MenuExit, EventHandler(fun s e -> form.Close()), Shortcut.AltF4)
    itemFile.MenuItems.Add(itemExit) |> ignore
    menu
