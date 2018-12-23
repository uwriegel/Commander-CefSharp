using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    class CustomProtocolFactory : ISchemeHandlerFactory
    {
        public const string SchemeName = "serve";

        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
            => new CustomProtocol();
    }
}
