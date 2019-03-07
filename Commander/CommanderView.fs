namespace Commander

open System
open System.IO

open CefSharp
open Model
open DirectoryProcessor

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
    let mutable currentSorting: (int*bool) option = None
    
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

    let processFile (file: string) processItemType = ()

    let sortBy items = 
        let ascendingOrDescending descending expression = 
            if descending then -expression else expression
            
        match currentSorting with
        | None -> items
        | Some currentSorting ->
            let sortFunction = 
                match currentSorting with
                | 0, descending -> fun a b -> ascendingOrDescending descending (String.Compare(a.Name, b.Name, true))
                | 1, descending -> fun a b -> ascendingOrDescending descending (String.Compare(a.Extension, b.Extension, true))
                | 2, descending -> fun a b -> ascendingOrDescending descending (if a.Date > b.Date then 1 else -1)
                | 3, descending -> fun a b -> ascendingOrDescending descending (int (a.Size - b.Size))
                | 4, descending -> fun a b -> ascendingOrDescending descending (FileVersion.compare (FileVersion.parse a.Version) (FileVersion.parse b.Version))
                | _ -> fun a b -> ascendingOrDescending false (String.Compare(a.Name, b.Name, true))
            {
                ViewType = items.ViewType
                Path = items.Path
                Drives = items.Drives
                Directories = items.Directories
                Files = 
                    items.Files
                    |> Array.sortWith sortFunction
            }

    let changePath path (directoryToSelect: string option) = 
        let request = requestFactory.create()
        let viewType = getViewType path
        let setColumns = viewType <> currentItems.ViewType 
        let get = 
            match viewType with
            | ViewType.Root ->
                currentSorting <- None
                RootProcessor.get
            | ViewType.Directory | _ ->
                DirectoryProcessor.get path Globals.showHidden
        async {
            let newItems = get ()

            if setColumns && not request.IsCancelled then
                let columns = getColumns viewType None
                let! response = browserAccess.executeScript "setColumns" (Some (columns:> obj))
                ()
            if not request.IsCancelled then  
                currentItems <- newItems
                browserAccess.setRecentPath currentItems.Path

                let getCurrentIndex () =
                    match directoryToSelect, viewType with
                    | None, _ -> ItemIndex.getDefault currentItems.ViewType
                    | Some value, ViewType.Directory -> 
                        let folderToSelect = 
                            newItems.Directories
                            |> Seq.tryFind (fun n -> String.Compare(n.Name, value, true) = 0)
                        match folderToSelect with
                        | Some value -> ItemIndex.create ItemType.Directory value.Index
                        | None -> 0
                    | Some value, ViewType.Root -> 
                        let folderToSelect = newItems.Drives |> Array.find (fun n -> String.Compare(n.Name, value, true) = 0)
                        ItemIndex.create ItemType.Directory folderToSelect.Index
                    | _, _ -> 0

                let! res = this.SetIndex (getCurrentIndex ())

                let! response = browserAccess.executeScript "itemsChanged" None
                if viewType = ViewType.Directory then
                    async {
                        let newItems =
                            if not request.IsCancelled then  
                                Some (extendItems currentItems)
                            else 
                                None
                        match request.IsCancelled, newItems with
                        | false, Some value -> 
                            currentItems <- sortBy value
                            let! response = browserAccess.executeScript "itemsChanged" None
                            ()
                        | _ -> ()
                        
                    } |> Async.Start
        } |> Async.Start
        ()

    let sort (index: int) (ascending: bool) =
        currentSorting <- Some (index, not ascending)
        currentItems <- sortBy currentItems
        browserAccess.executeScript "itemsChanged" None

    member this.Ready () = 
        changePath (browserAccess.getRecentPath ()) None 

    member this.ProcessItem processItemType = 
        let itemType = ItemIndex.getItemType currentIndex
        let item = getCurrentItemPath currentIndex

        match itemType, processItemType with
            | ItemType.Parent, _ -> 
                let info = new DirectoryInfo(currentItems.Path)
                changePath item (Some info.Name)
            | ItemType.Directory, ProcessItemType.Properties -> processFile item processItemType
            | ItemType.Directory, _ -> changePath item None
            | ItemType.File, _ -> processFile item processItemType
            | _ -> ()
        ()

    member this.GetItems () = 
        let responses = 
            match currentItems.ViewType, currentItems.Drives, currentItems.Directories, currentItems.Files with
            | ViewType.Root, drives, [||], [||] -> 
                drives
                |> Seq.mapi (fun i n -> createDriveResponse n.Name n.Label n.Size i (ItemIndex.isSelected currentIndex i ItemType.Directory))
                |> Seq.toArray
            | ViewType.Directory, [||], directories, files -> getItems currentIndex directories files
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

    member this.ChangePath path = changePath path None

    member this.Sort index ascending = sort index ascending

    member this.Copy (otherView: CommanderView) = ()
    member this.CreateFolder () = ()
    member this.AdaptPath (path: string) = ()
    member this.Path with get() = ""
    member this.Refresh () = changePath currentItems.Path None

    member this.GetTestItems() = DirectoryProcessor.getTestItems ()