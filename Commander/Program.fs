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
  
  // MENU:
  // Copy
  
  // TODO: a: does not extend to a:\
  // TODO: Set SetSelection in Items in F# to preserve selection while sorting 
  // TODO: Resizing smaller in PDF View with Grip not possible
  // TODO: CreateFolder: take selection name

