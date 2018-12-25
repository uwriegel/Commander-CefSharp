namespace Engine

type CommanderView(id: CommanderViewId, host: IHost) =
    let mutable viewType = Enums.ViewType.Root 

    let getViewType path = 
        if path = Root.Name then   
            Enums.Root
        else
            Enums.Directory

    member this.ready() = 
        let path = host.RecentPath
        this.changePath path

        let viewType = getViewType path
        match viewType with
        | Enums.Root -> Root.getColumns ()
        | Enums.Directory -> Directory.getColumns ()



        "Affenkopf"
                       
    member this.changePath path = 
        let newViewType = getViewType path
        let viewTypeChanged = newViewType <> viewType
        if viewTypeChanged then viewType <- newViewType

        let test = 
            match viewType with
            // TODO: async 
            | Enums.Root -> Root.getItems viewTypeChanged
            | Enums.Directory -> 3

        host.RecentPath <- path

    