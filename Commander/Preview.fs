namespace Commander

open System

module ClrWinApi = 
    open System
    open System.Runtime.InteropServices

    type FileFuncFlags =
        MOVE = 0x1
        | COPY = 0x2
        | DELETE = 0x3
        | RENAME = 0x4

    [<Flags>]
    type FileOpFlags = 
        MULTIDESTFILES = 0x1us
        | CONFIRMMOUSE = 0x2us
        /// <summary>
        /// Don't create progress/report
        /// </summary>
        | SILENT = 0x4us
        | RENAMEONCOLLISION = 0x8us
        /// <summary>
        /// Don't prompt the user.
        /// </summary>
        | NOCONFIRMATION = 0x10us
        /// <summary>
        /// Fill in SHFILEOPSTRUCT.hNameMappings.
        /// Must be freed using SHFreeNameMappings
        /// </summary>
        | WANTMAPPINGHANDLE = 0x20us
        | ALLOWUNDO = 0x40us
        /// <summary>
        /// On *.*, do only files
        /// </summary>
        | FILESONLY = 0x80us
        /// <summary>
        /// Don't show names of files
        /// </summary>
        | SIMPLEPROGRESS = 0x100us
        /// <summary>
        /// Don't confirm making any needed dirs
        /// </summary>
        | NOCONFIRMMKDIR = 0x200us
        /// <summary>
        /// Don't put up error UI
        /// </summary>
        | NOERRORUI = 0x400us
        /// <summary>
        /// Dont copy NT file Security Attributes
        /// </summary>
        | NOCOPYSECURITYATTRIBS = 0x800us
        /// <summary>
        /// Don't recurse into directories.
        /// </summary>
        | NORECURSION = 0x1000us
        /// <summary>
        /// Don't operate on connected elements.
        /// </summary>
        | NO_CONNECTED_ELEMENTS = 0x2000us
        /// <summary>
        /// During delete operation, 
        /// warn if nuking instead of recycling (partially overrides FOF_NOCONFIRMATION)
        /// </summary>
        | WANTNUKEWARNING = 0x4000us
        /// <summary>
        /// Treat reparse points as objects, not containers
        /// </summary>
        | NORECURSEREPARSE = 0x8000us

    /// <summary>
    /// [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    /// If you use the above you may encounter an invalid memory access exception (when using ANSI
    /// or see nothing (when using unicode) when you use FOF_SIMPLEPROGRESS flag.
    /// </summary>
    [<Struct; StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)>]
    type SHFILEOPSTRUCT = 
        val mutable Hwnd: IntPtr
        val mutable Func: FileFuncFlags
        [<MarshalAs(UnmanagedType.LPWStr)>]
        val mutable From: string
        [<MarshalAs(UnmanagedType.LPWStr)>]
        val mutable To: string
        val mutable Flags: FileOpFlags 
        [<MarshalAs(UnmanagedType.Bool)>]
        val mutable AnyOperationsAborted: bool
        val mutable NameMappings: IntPtr
        [<MarshalAs(UnmanagedType.LPWStr)>]
        val mutable ProgressTitle: String

    [<DllImport("kernel32.dll")>]
    extern UInt32 GetCurrentThreadId()

    [<DllImport("shell32.dll", CharSet = CharSet.Unicode)>]
    extern int SHFileOperation(SHFILEOPSTRUCT fileOp)

module Control =
    let deferredExecution<'a> (action: unit->'a) delayInMilliseconds (dispatcher: System.Windows.Forms.Control) = async {
        do! Async.Sleep delayInMilliseconds
        return dispatcher.Invoke(Func<'a>(action)) :?> 'a
    }
       

