using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;

namespace Commander.Browser
{
    class IconProtocol : ResourceHandler
    {
        public override bool ProcessRequestAsync(IRequest request, ICallback callback)
        {
            AutoDisposeStream = true;

            var ext = request.Url.Substring(7, request.Url.Length - 1 -7);
            SetIcon(ext, callback);
            return true;
        }

        async void SetIcon(string ext, ICallback callback)
        {
            Stream = await GetIconAsync(ext);
            MimeType = "image/png";
            StatusCode = 200;
            ResponseLength = Stream.Length;
            callback.Continue();
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