module Menu

open System.Windows.Forms
open Resources
open System

open Browser
open EnumerableExtensions   

type Themes = { Blue: string; LightBlue: string; Dark: string} 
let themes = { Blue = "blue"; LightBlue = "lightblue"; Dark = "dark" }

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

let itemZoom = new MenuItem(Resources.MenuZoom)

let clearZoomItems () =
    getSubMenuItems itemZoom.MenuItems |> Seq.forall (fun n -> n.Checked <- false; true) |> ignore

let getMenuItems (menuItems: Menu.MenuItemCollection) =
    getSubMenuItems menuItems
    |> Seq.collect (fun n -> getSubMenuItems n.MenuItems)

let createMenu (form: Form) (browser: Browser) browserForm =
    let menu = new MainMenu()
    let itemFile = new MenuItem(Resources.MenuFile)
    menu.MenuItems.Add itemFile |> ignore
    let itemRename = new MenuItem(Resources.MenuRename, EventHandler(fun s e -> ()), Shortcut.F2)
    itemFile.MenuItems.Add itemRename |> ignore
    itemFile.MenuItems.Add "-" |> ignore
    let itemCopy = new MenuItem(Resources.MenuCopy, EventHandler(fun s e -> browser.Copy ()), Shortcut.F5)
    itemFile.MenuItems.Add itemCopy |> ignore
    let itemMove = new MenuItem(Resources.MenuMove, EventHandler(fun s e -> ()), Shortcut.F6)
    itemFile.MenuItems.Add itemMove |> ignore
    let itemDelete = new MenuItem(Resources.MenuDelete, EventHandler(fun s e -> ()), Shortcut.Del)
    itemFile.MenuItems.Add itemDelete |> ignore
    itemFile.MenuItems.Add "-" |> ignore
    let itemCreateFolder = new MenuItem(Resources.MenuCreateFolder, EventHandler(fun s e -> browser.CreateFolder ()), Shortcut.F7)
    itemFile.MenuItems.Add itemCreateFolder |> ignore 
    let itemProperties = new MenuItem(Resources.MenuProperties, EventHandler(fun s e -> ()))
    itemFile.MenuItems.Add itemProperties |> ignore 
    itemFile.MenuItems.Add "-" |> ignore 
    let itemExit = new MenuItem(Resources.MenuExit, EventHandler(fun s e -> form.Close()), Shortcut.AltF4)
    itemFile.MenuItems.Add itemExit |> ignore

    let itemNavigation = new MenuItem(Resources.MenuNavigation)
    menu.MenuItems.Add itemNavigation |> ignore
    let itemFavourites = new MenuItem(Resources.MenuFavourites, EventHandler(fun s e -> ()), Shortcut.F1);
    itemNavigation.MenuItems.Add itemFavourites |> ignore
    let itemSameFolder = new MenuItem(Resources.MenuSameFolder, EventHandler(fun s e -> browser.AdaptPath()), Shortcut.F9);
    itemNavigation.MenuItems.Add itemSameFolder |> ignore

    let itemSelection = new MenuItem(Resources.MenuSelection)
    menu.MenuItems.Add itemSelection |> ignore
    let itemAll = new MenuItem(Resources.MenuAll, EventHandler(fun s e -> ()))
    itemSelection.MenuItems.Add itemAll |> ignore
    let itemUnselectAll = new MenuItem(Resources.MenuUnselectAll, EventHandler(fun s e -> ()))
    itemSelection.MenuItems.Add itemUnselectAll |> ignore

    let onShowHidden(menuItem: MenuItem) = 
        menuItem.Checked <- not menuItem.Checked
        browser.ShowHidden menuItem.Checked

    let itemView = new MenuItem(Resources.MenuView)
    menu.MenuItems.Add itemView |> ignore
    let itemShowHidden = new MenuItem(Resources.MenuShowHidden, EventHandler(fun s e -> onShowHidden (s :?> MenuItem)), Shortcut.CtrlH)
    itemShowHidden.Checked <- false
    itemView.MenuItems.Add itemShowHidden |> ignore
    let itemRefresh = new MenuItem(Resources.MenuRefresh, EventHandler(fun s e -> browser.Refresh()), Shortcut.CtrlR)
    itemView.MenuItems.Add itemRefresh |> ignore
    itemView.MenuItems.Add "-" |> ignore

    let onViewer (menuItem: MenuItem) = 
        menuItem.Checked <- not menuItem.Checked
        browser.OnViewer menuItem.Checked

    let itemPreview = new MenuItem(Resources.MenuPreview, EventHandler(fun s e -> onViewer (s :?> MenuItem)), Shortcut.F3)
    itemPreview.Checked <- false
    itemView.MenuItems.Add itemPreview |> ignore
    itemView.MenuItems.Add "-" |> ignore

    let itemTheme = new MenuItem(Resources.MenuThemes)
    itemView.MenuItems.Add itemTheme |> ignore
    itemView.MenuItems.Add "-" |> ignore
    
    let itemThemeBlue = new MenuItem(Resources.MenuThemeBlue)
    itemThemeBlue.RadioCheck <- true
    let itemThemeLightBlue = new MenuItem(Resources.MenuThemeLightBlue)
    itemThemeLightBlue.RadioCheck <- true
    let itemThemeDark = new MenuItem(Resources.MenuThemeDark)
    itemThemeDark.RadioCheck <- true

    if Settings.Default.Theme = themes.LightBlue then 
        itemThemeLightBlue.Checked <- true
    elif Settings.Default.Theme = themes.Dark then 
        itemThemeDark.Checked <- true
    else 
        itemThemeBlue.Checked <- true

    let onTheme (src: obj) (e: EventArgs) =
        if src.Equals itemThemeBlue then
            itemThemeBlue.Checked <- true
            itemThemeLightBlue.Checked <- false
            itemThemeDark.Checked <- false
            browser.SetTheme themes.Blue
            Settings.Default.Theme <- themes.Blue
        elif src.Equals itemThemeLightBlue then
            itemThemeBlue.Checked <- false
            itemThemeLightBlue.Checked <- true
            itemThemeDark.Checked <- false
            browser.SetTheme themes.LightBlue
            Settings.Default.Theme <- themes.LightBlue
        elif src.Equals itemThemeDark then
            itemThemeBlue.Checked <- false
            itemThemeLightBlue.Checked <- false
            itemThemeDark.Checked <- true
            browser.SetTheme themes.Dark
            Settings.Default.Theme <- themes.Dark
        Settings.Default.Save()

    itemThemeBlue.Click.AddHandler(EventHandler(onTheme))
    itemThemeLightBlue.Click.AddHandler(EventHandler(onTheme))
    itemThemeDark.Click.AddHandler(EventHandler(onTheme))

    itemThemeBlue.RadioCheck <- true
    itemThemeLightBlue.RadioCheck <- true
    itemThemeDark.RadioCheck <- true
    itemTheme.MenuItems.Add itemThemeBlue |> ignore
    itemTheme.MenuItems.Add itemThemeLightBlue |> ignore
    itemTheme.MenuItems.Add itemThemeDark |> ignore
           
    let onZoom (thisMenuItem: MenuItem, zoomLevel) = 
        browser.OnZoom zoomLevel
        clearZoomItems ()
        thisMenuItem.Checked <- true

    itemView.MenuItems.Add itemZoom |> ignore
    let itemZoom50 = new MenuItem("50%", EventHandler(fun s e -> onZoom(s :?> MenuItem, 50.0)))
    itemZoom50.RadioCheck <- true
    itemZoom.MenuItems.Add itemZoom50 |> ignore
    let itemZoom75 = new MenuItem("75%", EventHandler(fun s e -> onZoom(s :?> MenuItem, 75.0)))
    itemZoom75.RadioCheck <- true
    itemZoom.MenuItems.Add itemZoom75 |> ignore
    let itemZoom100 = new MenuItem("100%", EventHandler(fun s e -> onZoom(s :?> MenuItem, 100.0)))
    itemZoom100.Checked <- true
    itemZoom100.RadioCheck <- true
    itemZoom.MenuItems.Add itemZoom100 |> ignore
    let itemZoom150 = new MenuItem("150%", EventHandler(fun s e -> onZoom(s :?> MenuItem, 150.0)))
    itemZoom150.RadioCheck <- true
    itemZoom.MenuItems.Add itemZoom150 |> ignore
    let itemZoom200 = new MenuItem("200%", EventHandler(fun s e -> onZoom(s :?> MenuItem, 200.0)))
    itemZoom200.RadioCheck <- true
    itemZoom.MenuItems.Add itemZoom200 |> ignore
    let itemZoom250 = new MenuItem("250%", EventHandler(fun s e -> onZoom(s :?> MenuItem, 250.0)))
    itemZoom250.RadioCheck <- true
    itemZoom.MenuItems.Add itemZoom250 |> ignore
    let itemZoom300 = new MenuItem("300%", EventHandler(fun s e -> onZoom(s :?> MenuItem, 300.0)))
    itemZoom300.RadioCheck <- true
    itemZoom.MenuItems.Add itemZoom300 |> ignore
    let itemZoom350 = new MenuItem("350%", EventHandler(fun s e -> onZoom(s :?> MenuItem, 350.0)))
    itemZoom350.RadioCheck <- true
    itemZoom.MenuItems.Add itemZoom350 |> ignore
    let itemZoom400 = new MenuItem("400%", EventHandler(fun s e -> onZoom(s :?> MenuItem, 400.0)))
    itemZoom400.RadioCheck <- true
    itemZoom.MenuItems.Add itemZoom400 |> ignore

    let itemFullscreen = new MenuItem(Resources.MenuFullscreen, EventHandler(fun s e -> browserForm.ToFullScreen()), Shortcut.F11)
    itemView.MenuItems.Add itemFullscreen |> ignore
    itemView.MenuItems.Add "-" |> ignore
    let itemDevTools = new MenuItem(Resources.MenuDeveloperTools, EventHandler(fun s e -> browser.ShowDevTools ()), Shortcut.F12)
    itemView.MenuItems.Add itemDevTools |> ignore
    
    let accelerators = 
        getMenuItems menu.MenuItems
        |> Seq.map createAccelerator
        |> Seq.filter (fun n -> n.Key <> 0)
        |> Seq.toArray
    
    browser.InitializeAccelerators accelerators
    
    (menu, clearZoomItems)
