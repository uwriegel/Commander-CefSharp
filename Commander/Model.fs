module Model

open System
open System.Diagnostics

type ColumnsType = String = 0 | Size = 1 | Date = 2

type  ProcessItemType = Show = 0 | Properties = 1 | StartAs = 2

type Column = {
    Name: string
    IsSortable: bool
    ColumnsType: ColumnsType
}

[<NoComparison>]
type Columns = {
    Name: string 
    Values: seq<Column>
}

type ViewType = Uninitialized = -1 | Root = 0 | Directory = 1

type Drive = {
    Index: int
    Name: string
    Label: string
    Size: int64
}

type DirectoryItem = {
    Index: int
    Name: string
    Date: DateTime 
    IsHidden: bool
}

[<NoComparison>]
type FileItem = {
    Index: int 
    Name: string 
    Extension: string 
    Icon: string 
    Date: DateTime 
    Size: int64 
    IsHidden: bool 
    isExif: bool 
    Version: string 
}

let updateVersion fileItem version =
    {
        Index = fileItem.Index
        Name = fileItem.Name
        Extension = fileItem.Extension
        Icon = fileItem.Icon
        Date = fileItem.Date
        Size = fileItem.Size
        IsHidden = fileItem.IsHidden
        isExif = fileItem.isExif
        Version = version
    }

let updateExif fileItem exif = 
    let dateTime, hasExif = 
        match exif with
        | Some value -> value, true
        | None -> fileItem.Date, false
    {
        Index = fileItem.Index
        Name = fileItem.Name
        Extension = fileItem.Extension
        Icon = fileItem.Icon
        Date = dateTime
        Size = fileItem.Size
        IsHidden = fileItem.IsHidden
        isExif = hasExif
        Version = fileItem.Version
    }

[<NoComparison>]
type Items = {
    ViewType: ViewType
    Path: string
    Drives: Drive[] 
    Directories: DirectoryItem[] 
    Files: FileItem[] 
}

let createEmptyItems () = {ViewType = ViewType.Uninitialized; Path = ""; Drives = [||]; Directories = [||]; Files = [||] }

let createDriveItems drives = {ViewType = ViewType.Root; Path = "root"; Drives = drives; Directories = [||]; Files = [||] }

let createDirectoryItems path directories files = {ViewType = ViewType.Directory; Path = path; Drives = [||]; Directories = directories; Files = files }

type ItemType = Undefined = 0 | Parent = 1 | Directory = 2 | File = 3 | Drive = 4

[<NoComparison>]
type ResponseItem = {
    ItemType: ItemType 
    Index: int
    Items: seq<string>
    Icon: string
    IsCurrent: bool 
    IsHidden: bool 
    IsExif: bool
}

let createDriveResponse name label (size: int64) index isCurrent = {
    ItemType = ItemType.Directory
    Index = index
    Items = [ name; label; size.ToString "N0" ]
    Icon = "Drive"
    IsCurrent = isCurrent
    IsHidden = false
    IsExif = false
}

let createParentResponse itemIndex isCurrent = {
    ItemType = ItemType.Parent
    Index = itemIndex 
    Items = [ ".." ]
    Icon = "Folder"
    IsCurrent = isCurrent
    IsHidden = false
    IsExif = false
}
   
let createDirectoryResponse name (date: DateTime) index isCurrent isHidden = {
    ItemType = ItemType.Directory
    Index = index
    Items = [ name; ""; date.ToString "g" ]
    Icon = "Folder"
    IsCurrent = isCurrent
    IsHidden = isHidden 
    IsExif = false
}

let createFileResponse name ext (date: DateTime) (size: int64) version icon index isCurrent isHidden isExif = {
    ItemType = ItemType.File
    Index = index
    Items = [ name; ext; date.ToString "g"; size.ToString("N0"); version ]
    Icon = icon
    IsCurrent = isCurrent
    IsHidden = isHidden
    IsExif = isExif
}

[<NoComparison>]
type Response = {
    Path: string
    Items: seq<ResponseItem>
}

type Conflict = {
    Source: string 
    Target: string
    CompareResult: int
}

type ConflictItem = {
    Name: string
    Icon: string
    Size: Conflict
    Time: Conflict
    Version: Conflict option
}
