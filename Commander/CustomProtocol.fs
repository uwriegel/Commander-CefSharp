module CustomProtocol

open System

open CefSharp
open CefSharp.WinForms
open System.IO

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

        let test = request.Url.Substring 8
        if test.StartsWith "commander/icon?path=" then
            let ext = test.Substring 20
            //SetIcon(ext, callback)
            true
        elif test.StartsWith "commander/file?path=" then
            //let file = Uri.UnescapeDataString(test.Substring(20))
            //Serve(file, request.Url, callback);
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


