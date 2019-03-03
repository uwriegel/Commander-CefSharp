namespace Commander

module ClrWinApi = 
    open System
    open System.Runtime.InteropServices

    [<DllImport("kernel32.dll")>]
    extern UInt32 GetCurrentThreadId()


module FileVersion = 
    
    open System.Diagnostics

    let hasInfo (fvi: FileVersionInfo) = 
        fvi <> null && not (fvi.FileMajorPart = 0 && fvi.FileMinorPart = 0 && fvi.FileBuildPart = 0 && fvi.FilePrivatePart = 0)

    let getVersion (fvi: FileVersionInfo) = 
        if hasInfo fvi then 
            Some (string fvi.FileMajorPart + "." + string fvi.FileMinorPart + "." + string fvi.FileBuildPart + "." + string fvi.FilePrivatePart)
        else
            None

