open System.Windows.Forms

open Commander
open System

[<EntryPoint>]
[<STAThread>]
let main argv = 
    Cef.initialize ()
    Globals.initialize argv
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault false
    Application.Run(new MainForm())
    0 
 
 // TODO: Sorting in DirectoryProcessor not CommanderView
 // TODO: Sorting: selection is wrong
 // TODO: setSelected exception (Einfg)
 
  