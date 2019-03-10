module DirectoryProcessor

open System
open System.Diagnostics
open System.IO
open System.Windows.Forms

open EnumExtensions
open Model

open Commander
open ClrWinApi
open System.Text
open System.Runtime.InteropServices

[<Literal>]
let name = "directory"

let getIcon fullname extension = 
    let part1 = if Globals.isAngularServing then "serve://commander/" else "" 
    let part2 = if String.Compare(extension, ".exe", true) = 0 then "icon?path=" + fullname else "icon?path=" + extension
    part1 + part2

let getSafeItems get =
    try 
        get ()
    with
    | :? UnauthorizedAccessException -> [||]

let getFullName fileItem path =
    Path.Combine(path, fileItem.Name + fileItem.Extension)

let columns = [{ Name = Resources.Resources.DirectoryName; IsSortable = true; ColumnsType = ColumnsType.String };
               { Name = Resources.Resources.DirectoryExtension; IsSortable = true; ColumnsType = ColumnsType.String };  
               { Name = Resources.Resources.DirectoryDate; IsSortable = true; ColumnsType = ColumnsType.Date };  
               { Name = Resources.Resources.DirectorySize; IsSortable = true; ColumnsType = ColumnsType.Size };  
               { Name = Resources.Resources.DirectoryVersion; IsSortable = true; ColumnsType = ColumnsType.String }] 

let getNameOnly (name : string) = 
    let pos = name.LastIndexOf "."
    if pos <> -1 then name.Substring(0, pos) else name

let get path showHidden () =

    try 
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

        let files =
            getSafeItems (fun () -> di.GetFiles())
            |> Seq.filter (fun n -> 
                if showHidden then 
                    true 
                else 
                    not (hasFlag n.Attributes FileAttributes.Hidden)
            )
            |> Seq.mapi (fun i n -> {
                    Index = i
                    Name = getNameOnly n.Name
                    Extension = n.Extension
                    Size = n.Length
                    Date = n.LastWriteTime
                    isExif = false
                    IsHidden = hasFlag n.Attributes FileAttributes.Hidden 
                    Icon = getIcon n.Name n.Extension
                    Version = null
                }) 
            |> Seq.toArray

        createDirectoryItems path directories files

    with 
    | _ -> createEmptyItems ()

let getItems currentIndex (directories: DirectoryItem[]) (files: FileItem[]) = 
    let parent = [ createParentResponse (ItemIndex.create ItemType.Parent 0) (ItemIndex.isSelected currentIndex 0 ItemType.Parent) ] 

    let directories = 
        directories
        |> Seq.mapi (fun i n -> createDirectoryResponse n.Name n.Date (ItemIndex.create ItemType.Directory i) 
                                    (ItemIndex.isSelected currentIndex i ItemType.Directory) n.IsHidden)
        |> Seq.toList

    let files = 
        files
        |> Seq.mapi (fun i n -> createFileResponse n.Name n.Extension n.Date n.Size n.Version n.Icon (ItemIndex.create ItemType.File n.Index) 
                                                    (ItemIndex.isSelected currentIndex n.Index ItemType.File) n.IsHidden n.isExif)
        |> Seq.toList

    List.concat [ parent; directories; files ]
    |> List.toArray

let extendItem (itemToExtend: FileItem) (path: string) = 

    let updateVersion () = 
        let file = getFullName itemToExtend path
        let fvi =  FileVersionInfo.GetVersionInfo file
        match FileVersion.getVersion fvi with
        | Some value -> updateVersion itemToExtend value
        | None -> itemToExtend

    let updateExif () = 
        let exif = 
            getFullName itemToExtend path
            |> ExifReader.getExif
        match exif with 
        | Some reader -> updateExif itemToExtend (ExifReader.getDateValue ExifReader.ExifTag.DateTimeOriginal reader)
        | None -> itemToExtend

    if String.Compare(itemToExtend.Extension, ".tif", true) = 0 
            || String.Compare(itemToExtend.Extension, ".jpeg", true) = 0
            || String.Compare(itemToExtend.Extension, ".jpg", true) = 0 then 
        updateExif ()
    elif String.Compare(itemToExtend.Extension, ".exe", true) = 0 
            || String.Compare(itemToExtend.Extension, ".dll", true) = 0 then
        updateVersion ()
    else
        itemToExtend

let extendItems (itemsToExtend: Items) =
    let files = 
        itemsToExtend.Files
        |> Seq.map (fun n -> extendItem n itemsToExtend.Path)
        |> Seq.toArray
    { 
        Path = itemsToExtend.Path 
        ViewType = ViewType.Directory
        Drives = [||]
        Directories = itemsToExtend.Directories
        Files = files
    }

let getTestItems () = 
    let di = new DirectoryInfo(@"c:\windows\system32")
    di.GetFiles()
    |> Seq.map (fun n -> getIcon n.Name n.Extension)
    |> Seq.toArray

let sortBy currentSorting items = 
    let ascendingOrDescending descending expression = 
        if descending then -expression else expression
        
    match currentSorting with
    | None -> items
    | Some currentSorting ->
        let sortFunction = 
            match currentSorting with
            | 0, descending -> fun a b -> ascendingOrDescending descending (String.Compare(a.Name, b.Name, true))
            | 1, descending -> fun a b -> ascendingOrDescending descending (String.Compare(a.Extension, b.Extension, true))
            | 2, descending -> fun a b -> ascendingOrDescending descending (if a.Date > b.Date then 1 else -1)
            | 3, descending -> fun a b -> ascendingOrDescending descending (int (a.Size - b.Size))
            | 4, descending -> fun a b -> ascendingOrDescending descending (FileVersion.compare (FileVersion.parse a.Version) (FileVersion.parse b.Version))
            | _ -> fun a b -> ascendingOrDescending false (String.Compare(a.Name, b.Name, true))
        {
            ViewType = items.ViewType
            Path = items.Path
            Drives = items.Drives
            Directories = items.Directories
            Files = 
                items.Files
                |> Array.sortWith sortFunction
        }
    
let createFolder path item =
    let newFolder = Path.Combine(path, item)
    if Directory.Exists(newFolder) then 
        raise Exceptions.AlreadyExists

    try
        Directory.CreateDirectory newFolder |> ignore
    with :? UnauthorizedAccessException -> () // TODO: 

let getItemPathes (index, itemType) targetPath currentItems =
    match itemType with
    | ItemType.File -> 
        Some ((getFullName currentItems.Files.[index] currentItems.Path), 
            Path.Combine(targetPath, currentItems.Files.[index].Name + currentItems.Files.[index].Extension))
    | ItemType.Directory -> 
        Some (Path.Combine(currentItems.Path, currentItems.Directories.[index].Name),
            Path.Combine(targetPath, currentItems.Directories.[index].Name))
    | _ -> None 

let createFileOperationPaths (paths: seq<string>) =
    let sb = StringBuilder()
    paths |> Seq.iter (fun n -> sb.Append n |> ignore; sb.Append (char 0) |> ignore)
    sb.Append (char 0) |> ignore
    sb.ToString()

let copy (currentItems: Items) selectedItems (targetPath: string) (mainWindow: nativeint) (dispatcher: Control) = async {
    let pathes = selectedItems |> Seq.choose (fun n -> currentItems |> getItemPathes n targetPath)

    let mutable fileop = SHFILEOPSTRUCT()
    fileop.Flags <- FileOpFlags.NOCONFIRMATION ||| FileOpFlags.NOCONFIRMMKDIR ||| FileOpFlags.MULTIDESTFILES
    fileop.Hwnd <- mainWindow
    fileop.ProgressTitle <- "Commander"
    fileop.Func <- FileFuncFlags.COPY
    fileop.From <- createFileOperationPaths (pathes |> Seq.map(fun (source, _) -> source))
    fileop.To <- createFileOperationPaths (pathes |> Seq.map(fun (_, target) -> target))
    
    // Wait till animation has finished
    let! result = dispatcher |> Control.deferredExecution (fun () -> SHFileOperation fileop) 400
    return 
        match result with
        | 0 -> true
        | _ -> false
}

