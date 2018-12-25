module Directory

open Model

let Name = "directory"

let getColumns () = {
    name = Name
    values = [| 
        // TODO: take Browser language
        { name = "Name"; isSortable = true }
        { name = "Erw."; isSortable = true }
        { name = "Datum"; isSortable = true }
        { name = "Größe"; isSortable = true }
        { name = "Version"; isSortable = true }
    |]
}
