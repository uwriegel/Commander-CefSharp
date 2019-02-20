module DirectoryProcessor

open Model
open System.IO
open System

[<Literal>]
let name = "directory"

let getSafeItems get =
    try 
        get ()
    with
    | :? UnauthorizedAccessException -> [||]

let columns = [ { Name = Resources.Resources.DirectoryName; IsSortable = false; ColumnsType = ColumnsType.String };
                   { Name = Resources.Resources.DirectoryName; IsSortable = false; ColumnsType = ColumnsType.String };  
                   { Name = Resources.Resources.DirectoryExtension; IsSortable = false; ColumnsType = ColumnsType.String };  
                   { Name = Resources.Resources.DirectoryDate; IsSortable = true; ColumnsType = ColumnsType.Date };  
                   { Name = Resources.Resources.DirectorySize; IsSortable = true; ColumnsType = ColumnsType.Size };  
                   { Name = Resources.Resources.DirectoryVersion; IsSortable = false; ColumnsType = ColumnsType.String } 
               ] 

let get path showHidden =
    let di = new DirectoryInfo(path)
    let directories =
        getSafeItems (fun () -> di.GetDirectories())
        |> Seq.filter (fun n -> 
            if showHidden then 
                true 
            else 
                not (n.Attributes.HasFlag FileAttributes.Hidden)
        )
        |> Seq.mapi (fun i n -> ())
        //.Select((n, i) => new DirectoryItem(i, n.Name, n.LastWriteTime, n.Attributes.HasFlag(FileAttributes.Hidden)));

    createEmptyItems ()