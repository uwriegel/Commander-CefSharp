namespace Commander

open CefSharp
open Model
open System.Threading

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
    
    let getViewType (path: string) = 
        match path with 
        | RootProcessor.name -> ViewType.Root
        | _ when path.EndsWith("..") && path.Length = 5 -> ViewType.Root
        | _ -> ViewType.Directory

    let getColumns viewType = 
        match viewType with
        | ViewType.Root -> { Name = RootProcessor.name; Values = RootProcessor.columns }
        | ViewType.Directory | _ -> { Name = DirectoryProcessor.name; Values = DirectoryProcessor.columns }

    let changePath path = 
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
                let columns = getColumns viewType
                let! response = browserAccess.executeScript "setColumns" (Some (columns:> obj))
                ()
            if not request.IsCancelled then  
                currentItems <- newItems
                //sort()
                browserAccess.setRecentPath currentItems.Path
                let! response = browserAccess.executeScript "itemsChanged" None
                ()
        } |> Async.Start
        ()

    member this.Ready () = 
        let path = browserAccess.getRecentPath ()
        //let viewType = getViewType path
        changePath path
    
    member this.Copy (otherView: CommanderView) = ()
    member this.CreateFolder () = ()
    member this.AdaptPath (path: string) = ()
    member this.Path with get() = ""
    member this.ShowHidden (showHidden: bool) = ()
    member this.Refresh () = ()