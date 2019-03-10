namespace Commander

open System
open System.IO
open System.Windows.Forms

open CefSharp
open Model
open DirectoryProcessor
open System.Diagnostics
open ClrWinApi
open System.Runtime.InteropServices

[<NoComparison>]
[<NoEquality>]
type BrowserAccess = {
    GetRecentPath: unit->string
    SetRecentPath: string->unit
    ExecuteScript: string->obj option->Async<JavascriptResponse>
    ExecuteCommanderScript: string->obj option->Async<JavascriptResponse>
    ExecuteScriptWithParams: string->obj[]->Async<JavascriptResponse>
    MainWindow: nativeint
    Dispatcher: Control
}

type CommanderView(browserAccess: BrowserAccess) as this =
    let requestFactory = RequestFactory()
    let mutable currentItems = Model.createEmptyItems ()
    let mutable currentIndex = 0
    let mutable currentSorting: (int*bool) option = None
    let mutable selectedIndexes: int[] = [||]
    
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

    let processFile (file: string) processItemType = 
        match processItemType with
        | ProcessItemType.Show ->  
            use p = new Process()
            p.StartInfo.UseShellExecute <- true
            p.StartInfo.ErrorDialog <- true
            p.StartInfo.FileName <- file
            p.Start () |> ignore
        | ProcessItemType.Properties ->  
            let mutable info = ShellExecuteInfo()
            info.Verb <- "properties"
            info.File <- file
            info.Show <- ShowWindowFlag.Show
            info.Mask <- ShellExecuteFlag.InvokeIDList
            info.Size <- Marshal.SizeOf(info)
            ShellExecuteEx(info) |> ignore
        | ProcessItemType.StartAs ->  
            Process.Start("rundll32.exe", "shell32, OpenAs_RunDLL "+ file) |> ignore
        | _ -> ()

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
                let! response = browserAccess.ExecuteScript "setColumns" (Some (columns:> obj))
                ()
            if not request.IsCancelled then  
                currentItems <- newItems
                browserAccess.SetRecentPath currentItems.Path

                let getCurrentIndex () =
                    match directoryToSelect, viewType with
                    | None, ViewType.Root -> ItemIndex.create ItemType.Drive 0
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
                        folderToSelect.Index
                    | _, _ -> 0

                let! res = this.SetIndex (getCurrentIndex ())

                let! response = browserAccess.ExecuteScript "itemsChanged" None
                if viewType = ViewType.Directory then
                    async {
                        let newItems =
                            if not request.IsCancelled then  
                                Some (extendItems currentItems)
                            else 
                                None
                        match request.IsCancelled, newItems with
                        | false, Some value -> 
                            currentItems <- sortBy currentSorting value
                            let! response = browserAccess.ExecuteScript "itemsChanged" None
                            ()
                        | _ -> ()
                        
                    } |> Async.Start
        } |> Async.Start
        ()

    let sort (index: int) (ascending: bool) =
        currentSorting <- Some (index, not ascending)
        currentItems <- sortBy currentSorting currentItems
        browserAccess.ExecuteScript "itemsChanged" None

    let getSelectedItems () = 
        let selectedIndexes = 
            match selectedIndexes.Length with
            | 0 -> [| currentIndex |]
            | _ -> selectedIndexes

        selectedIndexes |> Seq.map (fun n -> (ItemIndex.getArrayIndex n, ItemIndex.getItemType n))

    member this.Ready () = 
        changePath (browserAccess.GetRecentPath ()) None 

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
            | ItemType.Drive, _ -> changePath item None
            | _ -> ()
        ()

    member val internal Other = this with get, set

    member this.GetItems () = 
        let responses = 
            match currentItems.ViewType, currentItems.Drives, currentItems.Directories, currentItems.Files with
            | ViewType.Root, drives, [||], [||] -> 
                drives
                |> Seq.mapi (fun i n -> createDriveResponse n.Name n.Label n.Size n.Index (ItemIndex.isSelected currentIndex i ItemType.Drive))
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
        browserAccess.ExecuteScript "setCurrentItem" (Some ((getCurrentItemPath currentIndex):>obj))

    member this.ChangePath path = changePath path None

    member this.Sort index ascending = sort index ascending

    member this.StartCopy (otherView: CommanderView) = 
        browserAccess.ExecuteScriptWithParams "copy" [| otherView.Path :> obj; Resources.Resources.dialogCopy :> obj |] |> ignore

    member this.StartCreateFolder () = 
        //TODO: dialog.inputText = item.items[0] != ".." ? item.items[0] : ""
        browserAccess.ExecuteScript "createFolder" (Some (Resources.Resources.dialogCreateFolder :> obj)) |> ignore

    member this.AdaptPath (path: string) = ()
    member this.Path with get() = currentItems.Path
    member this.Refresh () = changePath currentItems.Path None
    member this.SetSelected (selectedValues: obj[]) = 
        let toInt (n: obj) = 
            match n with
            | null -> 0
            | _ -> n :?> int
        match selectedValues with 
        | null -> selectedIndexes <- [||]
        | _ -> selectedIndexes <-  selectedValues |> Array.map toInt
        
    member this.CreateFolder (item: string) =
        try 
            match currentItems.ViewType with
            | ViewType.Directory -> 
                DirectoryProcessor.createFolder currentItems.Path item
                changePath currentItems.Path (Some item)
            | _ -> ()
        with Exceptions.AlreadyExists ->
            browserAccess.ExecuteCommanderScript "showDialog" (Some (Resources.Resources.FolderAlreadyExists :> obj)) |> ignore

    member this.Copy (targetPath: string) = 
        async { 
            let selectedItems = getSelectedItems ()
            let index, itemType = selectedItems |> Seq.head
            let! refresh = 
                match itemType with
                | ItemType.Directory | ItemType.File -> 
                    DirectoryProcessor.copy currentItems selectedItems targetPath browserAccess.MainWindow browserAccess.Dispatcher
                | _ -> async.Return false
            if refresh then this.Other.Refresh() 
        } |> Async.Start

    member this.GetTestItems() = DirectoryProcessor.getTestItems ()