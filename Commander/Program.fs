open System.Windows.Forms

open Commander
open System

[<EntryPoint>]
[<STAThread>]
let main argv = 


    let exif = ExifReader.getExif @"C:\Users\uwe.CASERIS\Pictures\bild03.jpg"
    let res = 
        match exif with
        | Some reader -> Some (ExifReader.getDateValue ExifReader.ExifTag.DateTimeOriginal reader)
        | None -> None

    Cef.initialize ()
    Globals.initialize argv
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault false
    Application.Run(new MainForm())
    0 
 
 // TODO: Sorting
 // TODO: Version
 // TODO: Exif