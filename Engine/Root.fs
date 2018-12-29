module Root

open Model
open System.IO

let Name = "root"


let get () = 

    let getResponseDriveItem index (item: DriveInfo) = { 
        itemType = ItemType.Directory
        index = index
        icon = "Drive"
        items = [| item.Name; item.VolumeLabel; string item.TotalSize |] 
        isCurrent = false
        isHidden = false
    }

    DriveInfo.GetDrives () 
    |> Array.filter (fun drive -> drive.IsReady)
    |> Array.sortBy (fun n -> n.Name)
    |> Array.mapi getResponseDriveItem
