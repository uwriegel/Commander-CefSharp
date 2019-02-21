open System.Windows.Forms

open Commander

[<EntryPoint>]
let main argv = 
    Cef.initialize ()
    Globals.initialize argv
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault false
    Application.Run(new MainForm())
    0 
 
 // TODO: WinApi nuget
 // TODO: Sorting
 // TODO: Version
 // TODO: Exif