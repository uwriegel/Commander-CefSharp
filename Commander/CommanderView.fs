namespace Commander

open Model

[<NoComparison>]
[<NoEquality>]
type Settings = {
    getRecentPath: unit->string
    setRecentPath: string->unit
}

type CommanderView(settings: Settings)  =
    
    let getViewType (path: string) = 
        match path with 
        | RootProcessor.name -> ViewType.Root
        | _ when path.EndsWith("..") && path.Length = 5 -> ViewType.Root
        | _ -> ViewType.Directory

    let getColumns viewType = 
        match viewType with
        | ViewType.Root -> { Name = RootProcessor.name; Values = RootProcessor.columns }
        | ViewType.Directory | _ -> { Name = DirectoryProcessor.name; Values = DirectoryProcessor.columns }

    member this.Ready () = 
        let viewType = getViewType <| settings.getRecentPath ()
        let columns = getColumns viewType
        //let jason = Json.serialize columns
        ()
    
    member this.Copy (otherView: CommanderView) = ()
    member this.CreateFolder () = ()
    member this.AdaptPath (path: string) = ()
    member this.Path with get() = ""
    member this.ShowHidden (showHidden: bool) = ()
    member this.Refresh () = ()