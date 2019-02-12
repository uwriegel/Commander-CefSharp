using CefSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    class CustomProtocol : ResourceHandler
    {
        public override bool ProcessRequestAsync(IRequest request, ICallback callback)
        {
            AutoDisposeStream = true;

            var test = request.Url.Substring(8);
            if (test.StartsWith("commander/icon?path="))
            {
                var ext = test.Substring(20);
                SetIcon(ext, callback);
                return true;
            }
            else if (test.StartsWith("commander/file?path="))
            {
                var file = Uri.UnescapeDataString(test.Substring(20));
                Serve(file, request.Url, callback);
                return true;
            }

            if (test.StartsWith("commander"))
            {
                var file = test.Substring(10);
                if (file.Length == 0)
                    file = "index.html";
                file = @"Commander\" + file;
                Serve(file, request.Url, callback);
                return true;
            }
            else
                return false;
        }

        string RetrieveMimeType(string url)
        {
            var pos = url.LastIndexOf(".");
            if (pos == -1)
                "text/html";
            else
                var ext = url.Substring(pos + 1).ToLower()
                ResourceHandler.GetMimeType(ext)
            }
        }

        async void Serve(string file, string url, ICallback callback)
        {
            await Task.Factory.StartNew(() =>
            {
                Stream = File.OpenRead(file);
                MimeType = RetrieveMimeType(url);
                StatusCode = 200;
                ResponseLength = Stream.Length;
                callback.Continue();
            });
        }

        async void SetIcon(string ext, ICallback callback)
        {
            await Task.Factory.StartNew(async () =>
            {
                Stream = await GetIconAsync(ext);
                MimeType = "image/png";
                StatusCode = 200;
                ResponseLength = Stream.Length;
                callback.Continue();
            });
        }

        async Task<Stream> GetIconAsync(string ext)
        {
            async Task<IntPtr> GetIconHandleAsync(int callCount)
            {
                var shinfo = new ShFileInfo();
                Api.SHGetFileInfo(ext, Api.FileAttributeNormal, ref shinfo, Marshal.SizeOf(shinfo),
                       SHGetFileInfoConstants.ICON
                        | SHGetFileInfoConstants.SMALLICON
                        | SHGetFileInfoConstants.USEFILEATTRIBUTES
                        | SHGetFileInfoConstants.TYPENAME);
                if (shinfo.IconHandle != IntPtr.Zero)
                    return shinfo.IconHandle;
                else if (callCount < 3)
                {
                    await Task.Delay(29);
                    return await GetIconHandleAsync(callCount + 1);
                }
                else
                    return Icon.ExtractAssociatedIcon(@"C:\Windows\system32\SHELL32.dll").Handle;
            }

            var iconHandle = await GetIconHandleAsync(0);
            using (var icon = Icon.FromHandle(iconHandle))
            using (var bitmap = icon.ToBitmap())
            {
                var ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                Api.DestroyIcon(iconHandle);
                return ms;
            }
        }

        public bool IsRequestedPathInsideFolder(DirectoryInfo path, DirectoryInfo folder) => true;
    }
}
