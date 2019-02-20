module DirectoryProcessor

open Model
open System.IO
open System

open EnumExtensions

[<Literal>]
let name = "directory"

let getSafeItems get =
    try 
        get ()
    with
    | :? UnauthorizedAccessException -> [||]

let columns = [ { Name = Resources.Resources.DirectoryName; IsSortable = false; ColumnsType = ColumnsType.String };
                   { Name = Resources.Resources.DirectoryExtension; IsSortable = false; ColumnsType = ColumnsType.String };  
                   { Name = Resources.Resources.DirectoryDate; IsSortable = true; ColumnsType = ColumnsType.Date };  
                   { Name = Resources.Resources.DirectorySize; IsSortable = true; ColumnsType = ColumnsType.Size };  
                   { Name = Resources.Resources.DirectoryVersion; IsSortable = false; ColumnsType = ColumnsType.String } 
               ] 

let get path showHidden () =

    let di = new DirectoryInfo(path)
    let directories =
        getSafeItems (fun () -> di.GetDirectories())
        |> Seq.filter (fun n -> 
            if showHidden then 
                true 
            else 
                not (hasFlag n.Attributes FileAttributes.Hidden)
        )
        |> Seq.mapi (fun i n -> {Index = i; Name = n.Name; Date = n.LastWriteTime; IsHidden = hasFlag n.Attributes FileAttributes.Hidden }) 
        |> Seq.toArray

    createDirectoryItems path directories

let getItems currentIndex (directories: DirectoryItem[]) (files: FileItem[]) = 
    let parent = [ createParentResponse (ItemIndex.create ItemType.Parent 0) (ItemIndex.isSelected currentIndex 0 ItemType.Parent) ] 

    let directories = 
        directories
        |> Seq.mapi (fun i n -> createDirectoryResponse n.Name n.Date (ItemIndex.create ItemType.Directory 0) (ItemIndex.isSelected currentIndex 0 ItemType.Directory))
        |> Seq.toList

    let files = 
        files
        |> Seq.mapi (fun i n -> createFileResponse n.Name n.Extension n.Date n.Size n.Icon (ItemIndex.create ItemType.Directory 0) (ItemIndex.isSelected currentIndex 0 ItemType.Directory))
        |> Seq.toList

    List.concat [ parent; directories; files ]
    |> List.toArray
