module Directory

open Model
open System.IO
open System

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

let get path = 

    let getSafeItems getItems =
        try 
            getItems ()
        with | :? UnauthorizedAccessException -> [||]   

    let dateTimeMinTicks = (DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks

    let convertTime (dateTime: DateTime) = 
        let jsDataTime = (dateTime.ToUniversalTime().Ticks - dateTimeMinTicks) / 10000L
        jsDataTime.ToString ()


    let createParentItem () = {
        itemType = ItemType.Parent
        index = 0
        icon = "Folder"
        items = [| ".."; ""; ""; ""; "" |] 
        isHidden = false
        isCurrent = false
    }

    let isHidden (attributes: FileAttributes) = attributes.HasFlag FileAttributes.Hidden
    
    let directoryInfo = DirectoryInfo path
    let directoryItems () = getSafeItems directoryInfo.GetDirectories 
    let fileItems () = getSafeItems directoryInfo.GetFiles

    let createDirectoryItem (item: DirectoryInfo) = { 
            itemType = ItemType.Directory
            index = 0
            icon = "Folder"
            items = [| item.Name; ""; convertTime item.LastWriteTime; ""; "" |] 
            isHidden = isHidden item.Attributes
            isCurrent = false
        }

    let getNameOnly name =
        match name with 
        | ".." -> name
        | _ -> 
            let pos = Str.lastIndexOf name "."
            match pos with
            | -1 -> name
            | _ -> Str.substringLength 0 pos name

    let createFileItem (item: FileInfo) = { 
            itemType = ItemType.File
            index = 0
            icon = 
                match Str.toLower item.Extension with
                | ".exe" -> sprintf "/request/icon?path=%s" item.FullName
                | _ -> sprintf "/request/icon?path=%s" item.Extension
            items = [| getNameOnly item.Name; item.Extension; convertTime item.LastWriteTime; string item.Length; "" |] 
            isHidden = isHidden item.Attributes
            isCurrent = false
        }

    let directoryItems = 
        directoryItems () 
        |> Array.map createDirectoryItem

    let fileItems = 
        fileItems ()
        |> Array.map createFileItem 

    let fillIndexes index (responseItem: ResponseItem)  =
        { 
            responseItem with
                index = index
        }

    Array.concat [| [| createParentItem () |] ; directoryItems ; fileItems |]
    |> Array.mapi fillIndexes