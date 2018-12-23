using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            if (test.StartsWith("commander"))
            {
                var file = test.Substring(10);
                if (file.Length == 0)
                    file = "index.html";
                file = @"Commander\" + file;
                Stream = File.OpenRead(file);
                MimeType = RetrieveMimeType(request.Url);
                StatusCode = 200;

                ResponseLength = Stream.Length;

                callback.Continue();
                return true;
            }
            else
                return false;
        }

        string RetrieveMimeType(string url)
        {
            var pos = url.LastIndexOf(".");
            if (pos == -1)
                return "text/html";
            else
            {
                var ext = url.Substring(pos + 1).ToLower();
                return GetMimeType(ext);
            }
        }

        public bool IsRequestedPathInsideFolder(DirectoryInfo path, DirectoryInfo folder) => true;
    }
}
