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
  