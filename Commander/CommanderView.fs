namespace Commander

open CefSharp
open Model
open System.Threading
open System.IO

[<NoComparison>]
[<NoEquality>]
type BrowserAccess = {
    getRecentPath: unit->string
    setRecentPath: string->unit
    executeScript: string->obj option->Async<JavascriptResponse>
}

type CommanderView(browserAccess: BrowserAccess)  =
    let requestFactory = RequestFactory()
    let mutable currentItems = Model.createEmptyItems ()
    let mutable currentIndex = 0
    
    let getViewType (path: string) = 
        match path with 
        | RootProcessor.name -> ViewType.Root
        | _ when path.EndsWith("..") && path.Length = 5 -> ViewType.Root
        | _ -> ViewType.Directory

    let getColumns viewType (directoryToSelect: string option) = 
        match viewType with
        | ViewType.Root -> { Name = RootProcessor.name; Values = RootProcessor.columns }
        | ViewType.Directory | _ -> { Name = DirectoryProcessor.name; Values = DirectoryProcessor.columns }

    let getCurrentItemPath index = 
        let itemType = ItemIndex.getItemType index
        let arrayIndex = ItemIndex.getArrayIndex index

        let getDirectoryItemPath () = 
            match itemType with
            | ItemType.Directory -> currentItems.Directories.[arrayIndex].Name
            | ItemType.File -> currentItems.Files.[arrayIndex].Name + currentItems.Files.[arrayIndex].Extension
            | ItemType.Parent ->
                let info = new DirectoryInfo(currentItems.Path)
                if info.Parent <> null then 
                    info.Parent.FullName 
                else 
                    "root"
            | _ -> null
    
        let getDirectory () =
            let directory = getDirectoryItemPath ()
            if directory = "root" then 
                "root" 
            else System.IO.Path.Combine(currentItems.Path, directory)

        if currentItems.ViewType = ViewType.Root then
            currentItems.Drives.[arrayIndex].Name 
        else
            getDirectory ()

    let setIndex index =
        currentIndex <- index
        async {
            return! browserAccess.executeScript "setCurrentItem" (Some ("":>obj))
        }//await ExecuteScriptAsync("setCurrentItem", GetCurrentItemPath(currentIndex));


    let changePath path (directoryToSelect: string option)= 
        let request = requestFactory.create()
        let viewType = getViewType path
        let setColumns = viewType <> currentItems.ViewType 
        let get = 
            match viewType with
            | ViewType.Root ->
                //currentSorting <- None
                RootProcessor.get
            | ViewType.Directory | _ ->
                DirectoryProcessor.get
        async {
            let newItems = get ()

            if setColumns && not request.IsCancelled then
                let columns = getColumns viewType None
                let! response = browserAccess.executeScript "setColumns" (Some (columns:> obj))
                ()
            if not request.IsCancelled then  
                currentItems <- newItems
                //sort()
                browserAccess.setRecentPath currentItems.Path

                let getCurrentIndex () =
                    match directoryToSelect, viewType with
                    | None, _ -> ItemIndex.getDefault currentItems.ViewType
                    | Some value, ViewType.Directory -> 0
                    | Some value, ViewType.Root -> 
                        //var folderToSelect = newItems.Drives.First(n => string.Compare(n.Name, directoryToSelect, true) == 0);
                        //return ItemIndex.Create(ItemType.Directory, folderToSelect.Index);
                        0
                    | _, _ -> 0

                let! res = setIndex (getCurrentIndex ())

                let! response = browserAccess.executeScript "itemsChanged" None
                ()
        } |> Async.Start
        ()

    member this.Ready () = 
        changePath (browserAccess.getRecentPath ()) None 

    member this.GetItems () = 
        match currentItems.ViewType, currentItems.Drives, currentItems.Directories, currentItems.Files with
        | ViewType.Root, drives, [||], [||] -> ()
        | ViewType.Directory, [||], directories, files -> ()
        | _ -> ()

    member this.Copy (otherView: CommanderView) = ()
    member this.CreateFolder () = ()
    member this.AdaptPath (path: string) = ()
    member this.Path with get() = ""
    member this.ShowHidden (showHidden: bool) = ()
    member this.Refresh () = ()