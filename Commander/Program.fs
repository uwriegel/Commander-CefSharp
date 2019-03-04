open System.Windows.Forms

open Commander
open System

[<EntryPoint>]
[<STAThread>]
let main argv = 


    let affe = ExifReader.GetExif @"C:\Users\uwe.CASERIS\Pictures\bild02.jpg"
    let res = 
        match affe with
        | Some reader -> reader.GetTagValue (uint16 ExifReader.ExifTags.DateTimeOriginal)
        | None -> null


    Cef.initialize ()
    Globals.initialize argv
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault false
    Application.Run(new MainForm())
    0 
 
 // TODO: Sorting
 // TODO: Version
 // TODO: Exif