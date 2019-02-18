namespace Commander

open System
open System.IO
open System.Threading

open CefSharp
open Model

[<NoComparison>]
[<NoEquality>]
type BrowserAccess = {
    getRecentPath: unit->string
    setRecentPath: string->unit
    executeScript: string->obj option->Async<JavascriptResponse>
}

type CommanderView(browserAccess: BrowserAccess) as this =
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

    let changePath path (directoryToSelect: string option) = 
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
                // TODO:
                //sort()
                browserAccess.setRecentPath currentItems.Path

                let getCurrentIndex () =
                    match directoryToSelect, viewType with
                    | None, _ -> ItemIndex.getDefault currentItems.ViewType
                    | Some value, ViewType.Directory -> 0
                    | Some value, ViewType.Root -> 
                        let folderToSelect = newItems.Drives |> Array.find (fun n -> String.Compare(n.Name, value, true) = 0)
                        ItemIndex.create ItemType.Directory folderToSelect.Index
                    | _, _ -> 0

                let! res = this.SetIndex (getCurrentIndex ())

                let! response = browserAccess.executeScript "itemsChanged" None
                ()
        } |> Async.Start
        ()

    member this.Ready () = 
        changePath (browserAccess.getRecentPath ()) None 

    member this.GetItems () = 
        let responses = 
            match currentItems.ViewType, currentItems.Drives, currentItems.Directories, currentItems.Files with
            | ViewType.Root, drives, [||], [||] -> 
                drives
                |> Seq.mapi (fun i n -> createDriveResponse n.Name n.Label n.Size i (ItemIndex.isSelected currentIndex i ItemType.Directory))
            | ViewType.Directory, [||], directories, files -> failwith "not implemented"
            | _ -> failwith "Invalid ViewType"
        let response: Response = { 
            Path = currentItems.Path
            Items = responses 
        }
        Json.serialize response

    member this.SetIndex index =
        currentIndex <- index
        async {
            return! browserAccess.executeScript "setCurrentItem" (Some ((getCurrentItemPath currentIndex):>obj))
        }

    member this.Copy (otherView: CommanderView) = ()
    member this.CreateFolder () = ()
    member this.AdaptPath (path: string) = ()
    member this.Path with get() = ""
    member this.ShowHidden (showHidden: bool) = ()
    member this.Refresh () = ()