module Root

open Model

let Name = "root"

let getColumns () = {
        name = Name
        values = [| 
            { name = "Name"; isSortable = true }
            { name = "Bezeichnung"; isSortable = true }
            { name = "Größe"; isSortable = true }
        |]                
    }

let getItems viewTypeChanged = 

    if viewTypeChanged then 
        let columns = getColumns ()
        ()
    9

