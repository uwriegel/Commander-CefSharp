module Model

open System
open System.Diagnostics

type ColumnsType = String = 0 | Size = 1 | Date = 2

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
    //public static FileItem UpdateDate(FileItem itemToUpdate, DateTime date) => new FileItem(itemToUpdate, date);

    //public static FileItem UpdateVersion(FileItem itemToUpdate, FileVersionInfo version) => new FileItem(itemToUpdate, version);

    //public FileItem(int index, string name, string fullname, string extension, DateTime date, long size, bool isHidden)
    //{
    //    Index = index;
    //    Name = name.GetNameOnly();
    //    Extension = extension;
    //    Date = date;
    //    HasExifDate = false;
    //    Size = size;
    //    IsHidden = isHidden;
    //    Version = null;

    //    Icon = (Program.IsAngularServing ? "serve://commander/" : "") +
    //        (string.Compare(extension, ".exe", true) == 0 ? "icon?path=" + fullname : "icon?path=" + extension);
    //}

    //FileItem(FileItem itemToUpdate, DateTime date)
    //{
    //    Index = itemToUpdate.Index;
    //    Name = itemToUpdate.Name;
    //    Extension = itemToUpdate.Extension;
    //    HasExifDate = true;
    //    Date = date;
    //    Size = itemToUpdate.Size;
    //    IsHidden = itemToUpdate.IsHidden;
    //    Icon = itemToUpdate.Icon;
    //    Version = null;
    //}

    //FileItem(FileItem itemToUpdate, FileVersionInfo version)
    //{
    //    Index = itemToUpdate.Index;
    //    Name = itemToUpdate.Name;
    //    Extension = itemToUpdate.Extension;
    //    HasExifDate = false;
    //    Date = itemToUpdate.Date;
    //    Size = itemToUpdate.Size;
    //    IsHidden = itemToUpdate.IsHidden;
    //    Icon = itemToUpdate.Icon;
    //    Version = version;
    //}

    Index: int 
    Name: string 
    Extension: string 
    Icon: string 
    Date: DateTime 
    Size: int64 
    IsHidden: bool 
    HasExifDate: bool 
    Version: FileVersionInfo 
}

[<NoComparison>]
type Items = {
    //public static Items UpdateFiles(Items itemsToUpdate, IEnumerable<FileItem> files)
    //    => new Items(itemsToUpdate, files);

    //public Items(string path, IEnumerable<DirectoryItem> directories, IEnumerable<FileItem> files)
    //{
    //    Path = path;
    //    ViewType = ViewType.Directory;
    //    Drives = null;
    //    Directories = directories.ToArray();
    //    Files = files.ToArray();
    //}


    //Items(Items itemsToUpdate, IEnumerable<FileItem> files)
    //{
    //    Path = itemsToUpdate.Path;
    //    ViewType = ViewType.Directory;
    //    Drives = null;
    //    Directories = itemsToUpdate.Directories;
    //    Files = files.ToArray();
    //}

    ViewType: ViewType
    Path: string
    Drives: Drive[] 
    Directories: DirectoryItem[] 
    Files: FileItem[] 
}

let createEmptyItems () = {ViewType = ViewType.Uninitialized; Path = ""; Drives = [||]; Directories = [||]; Files = [||] }

let createDriveItems drives = {ViewType = ViewType.Root; Path = "root"; Drives = drives; Directories = [||]; Files = [||] }


type ItemType = Undefined = -1 | Parent = 0 | Directory = 1 | File = 2

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

let createDriveResponse name label (size: int64) index = {
    ItemType = ItemType.Directory
    Index = index
    Items = [ name; label; size.ToString "N0" ]
    Icon = "Drive"
    IsCurrent = false
    IsHidden = false
    IsExif = false
}
    
[<NoComparison>]
type Response = {
    //string ItemToSelect
    Path: string
    Items: seq<ResponseItem>
}
