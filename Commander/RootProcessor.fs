module RootProcessor

open System.IO

open Model


[<Literal>]
let name = "root"

let columns = [ { Name = Resources.Resources.RootName; IsSortable = false; ColumnsType = ColumnsType.String }; 
    { Name = Resources.Resources.RootLabel; IsSortable = false; ColumnsType = ColumnsType.String }; 
    { Name = Resources.Resources.RootSize; IsSortable = false; ColumnsType = ColumnsType.Size } 
] 

let get path =
    DriveInfo.GetDrives ()
    |> Seq.filter (fun n -> n.IsReady)
    |> Seq.sortBy (fun n -> n.Name)
    |> Seq.mapi (fun i n -> {
        Index = i
        Name = n.Name
        Label = n.VolumeLabel
        Size = n.TotalSize
    })
    |> Seq.toArray 
    |> createDriveItems 
