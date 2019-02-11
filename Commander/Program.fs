open System.Windows.Forms

open Commander

[<EntryPoint>]
let main argv = 
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault false
    Application.Run(new MainForm())
    0 
 