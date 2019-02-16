namespace Commander

open Model
open System.Threading

[<NoComparison>]
[<NoEquality>]
type Settings = {
    getRecentPath: unit->string
    setRecentPath: string->unit
}

type CommanderView(settings: Settings, executeScript: string->obj->unit)  =
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
            let affe = newItems.Drives.Value |> Seq.toArray

            if not request.IsCancelled then
                ()
        } |> Async.Start
        ()

    member this.Ready () = 
        let path = settings.getRecentPath ()
        let viewType = getViewType path
        let columns = getColumns viewType
        executeScript "setColumns" columns
        changePath path
    
    member this.Copy (otherView: CommanderView) = ()
    member this.CreateFolder () = ()
    member this.AdaptPath (path: string) = ()
    member this.Path with get() = ""
    member this.ShowHidden (showHidden: bool) = ()
    member this.Refresh () = ()