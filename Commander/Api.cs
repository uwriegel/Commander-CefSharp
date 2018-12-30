using System;
using System.Runtime.InteropServices;

namespace Commander
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct ShFileInfo
    {
        public IntPtr IconHandle;
        public int Icon;
        public UInt32 Attributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string DisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string TypeName;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct ShellExecuteInfo
    {
        public int cbSize;
        public uint fMask;
        public IntPtr hwnd;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpVerb;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpFile;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpParameters;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpDirectory;
        public int nShow;
        public IntPtr hInstApp;
        public IntPtr lpIDList;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpClass;
        public IntPtr hkeyClass;
        public uint dwHotKey;
        public IntPtr hIcon;
        public IntPtr hProcess;
    }

    [Flags]
    enum SHGetFileInfoConstants
    {
        ICON = 0x100,                // get icon
        DISPLAYNAME = 0x200,         // get display name
        TYPENAME = 0x400,            // get type name
        ATTRIBUTES = 0x800,          // get attributes
        ICONLOCATION = 0x1000,       // get icon location
        EXETYPE = 0x2000,            // return exe type
        SYSICONINDEX = 0x4000,       // get system icon index
        LINKOVERLAY = 0x8000,        // put a link overlay on icon
        SELECTED = 0x10000,          // show icon in selected state
        ATTRSPECIFIED = 0x20000,     // get only specified attributes
        LARGEICON = 0x0,             // get large icon
        SMALLICON = 0x1,             // get small icon
        OPENICON = 0x2,              // get open icon
        SHELLICONSIZE = 0x4,         // get shell size icon
        PIDL = 0x8,                  // pszPath is a pidl
        USEFILEATTRIBUTES = 0x10,    // use passed dwFileAttribute
        ADDOVERLAYS = 0x000000020,   // apply the appropriate overlays
        OVERLAYINDEX = 0x000000040,   // Get the index of the overlay
    }

    static class Api
    {
        public const int FileAttributeNormal = 0x80;
        public const int SW_SHOW = 5;
        public const uint SEE_MASK_INVOKEIDLIST = 12;

        [DllImport("shell32")]
        extern internal static IntPtr SHGetFileInfo(string pszPath, int dwFileAttributes, ref ShFileInfo psfi, int cbFileInfo, SHGetFileInfoConstants uFlags);

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ShellExecuteEx(ref ShellExecuteInfo lpExecInfo);

        [DllImport("user32.dll", SetLastError = true)]
        extern internal static bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        public static extern int SetActiveWindow(IntPtr hwnd);
    }
}
