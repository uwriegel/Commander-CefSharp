namespace Commander

module ClrWinApi = 
    open System
    open System.Runtime.InteropServices

    [<DllImport("kernel32.dll")>]
    extern UInt32 GetCurrentThreadId()


