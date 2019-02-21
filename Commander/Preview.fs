module ClrWinApi 

open System
open System.Runtime.InteropServices

[<StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)>]
type ShFileInfo =
    struct
        val mutable IconHandle: nativeint
        val mutable Icon: int
        val mutable Attributes: UInt32
        [<MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)>]
        val mutable DisplayName: string 
        [<MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)>]
        val mutable TypeName: string 
    end

[<Flags>]
type SHGetFileInfoConstants = 
    ICON = 0x100                 // get icon
    | DISPLAYNAME = 0x200        // get display name
    | TYPENAME = 0x400           // get type name
    | ATTRIBUTES = 0x800         // get attributes
    | ICONLOCATION = 0x1000      // get icon location
    | EXETYPE = 0x2000           // return exe type
    | SYSICONINDEX = 0x4000      // get system icon index
    | LINKOVERLAY = 0x8000       // put a link overlay on icon
    | SELECTED = 0x10000         // show icon in selected state
    | ATTRSPECIFIED = 0x20000    // get only specified attributes
    | LARGEICON = 0x0            // get large icon
    | SMALLICON = 0x1            // get small icon
    | OPENICON = 0x2             // get open icon
    | SHELLICONSIZE = 0x4        // get shell size icon
    | PIDL = 0x8                 // pszPath is a pidl
    | USEFILEATTRIBUTES = 0x10   // use passed dwFileAttribute
    | ADDOVERLAYS = 0x000000020  // apply the appropriate overlays
    | OVERLAYINDEX = 0x000000040 // Get the index of the overlay

let FileAttributeNormal = 0x80

[<DllImport("shell32")>]
extern nativeint SHGetFileInfo(string pszPath, int dwFileAttributes, ShFileInfo& psfi, int cbFileInfo, SHGetFileInfoConstants uFlags)
[<DllImport("user32.dll", SetLastError = true)>]
extern bool DestroyIcon(nativeint hIcon)
