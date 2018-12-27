module Engine

let GetViewType path = 
    if path = Root.Name then   
        Enums.Root
    else
        Enums.Directory

let GetColumns viewType =
    match viewType with
    | Enums.Root -> Root.getColumns ()
    | Enums.Directory -> Directory.getColumns ()
        
let Get viewType (path: string) = 
    match viewType with
    | Enums.Root -> Root.get ()
    | Enums.Directory -> Directory.get path

//    let mutable viewType = Enums.ViewType.Root 


//    member this.ready() = 
//        let path = host.RecentPath
//        this.changePath path

//        let viewType = getViewType path



//        "Affenkopf"
                       
//    member this.changePath path = 
//        let newViewType = getViewType path
//        let viewTypeChanged = newViewType <> viewType
//        if viewTypeChanged then viewType <- newViewType

//        let test = 
//            match viewType with
//            // TODO: async 
//            | Enums.Root -> Root.getItems viewTypeChanged
//            | Enums.Directory -> 3

//        host.RecentPath <- path

    