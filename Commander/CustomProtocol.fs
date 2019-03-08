module CustomProtocol

open System
open System.Drawing
open System.Drawing.Imaging
open System.IO
open System.Runtime.InteropServices

open CefSharp
open CefSharp.WinForms
open ClrWinApi

type CustomProtocol() =
    inherit ResourceHandler()
    override this.ProcessRequestAsync(request: IRequest, callback: ICallback): bool = 
        this.AutoDisposeStream <- true

        let retrieveMimeType (url: string) =
            let pos = url.LastIndexOf "."
            if pos = -1 then
                "text/html"
            else
                let ext = url.Substring(pos + 1).ToLower()
                ResourceHandler.GetMimeType ext

        let serve (file: string) (url: string) (callback: ICallback) =
            async {
                this.Stream <- File.OpenRead file
                this.MimeType <- retrieveMimeType url
                this.StatusCode <- 200
                this.ResponseLength <- Nullable<int64> this.Stream.Length
                callback.Continue()
            } |> Async.Start

        let asyncGetIcon (ext: string) = async {
            let rec asyncGetIconHandle callCount = async {
                let mutable shinfo = ShFileInfo()
                SHGetFileInfo(ext, FileAttributeNormal, &shinfo, Marshal.SizeOf shinfo,
                    SHGetFileInfoConstants.ICON
                    ||| SHGetFileInfoConstants.SMALLICON
                    ||| SHGetFileInfoConstants.USEFILEATTRIBUTES
                    ||| SHGetFileInfoConstants.TYPENAME) |> ignore

                if shinfo.IconHandle <> IntPtr.Zero then
                    return shinfo.IconHandle
                elif callCount < 3 then
                    do! Async.Sleep 29
                    return! asyncGetIconHandle <| callCount + 1
                else
                    return Icon.ExtractAssociatedIcon(@"C:\Windows\system32\SHELL32.dll").Handle
            }
            let! iconHandle = asyncGetIconHandle 0
            use icon = Icon.FromHandle iconHandle
            use bitmap = icon.ToBitmap()
            let ms = new MemoryStream()
            bitmap.Save(ms, ImageFormat.Png)
            ms.Position <- 0L
            DestroyIcon iconHandle |> ignore
            return ms
        }

        let asyncSetIcon (ext: string) (callback: ICallback) = async {
            let! stream = asyncGetIcon ext
            this.Stream <- stream
            this.MimeType <- "image/png"
            this.StatusCode <- 200
            this.ResponseLength <- Nullable<int64> this.Stream.Length
            callback.Continue()
        } 

        let test = request.Url.Substring 8
        if test.StartsWith "commander/icon?path=" then
            let ext = test.Substring 20
            async { do! asyncSetIcon ext callback } |> Async.Start 
            true
        elif test.StartsWith "commander/file?path=" then
            let file = Uri.UnescapeDataString(test.Substring(20))
            serve file request.Url callback
            true
        elif test.StartsWith "commander" then
            let mutable file = test.Substring 10
            if file.Length = 0 then file <- "index.html"
            file <- @"Commander\" + file
            serve file request.Url callback
            true
        else
            false

type CustomProtocolFactory() = 
    interface ISchemeHandlerFactory with
        member this.Create(browser: IBrowser, frame: IFrame, schemeName: string, request: IRequest): IResourceHandler = 
            new CustomProtocol() :> IResourceHandler

type CefSettings with
    member x.registerCustomProtocolFactory() = 
        let schemeFactory = CustomProtocolFactory()

        let customScheme = 
            CefCustomScheme(SchemeName = "serve",
                SchemeHandlerFactory = CustomProtocolFactory(),
                IsCSPBypassing = true
            )

        x.RegisterScheme customScheme


