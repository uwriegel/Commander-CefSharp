using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace Commander.Browser
{
    class IconProtocolFactory : ISchemeHandlerFactory
    {
        public const string SchemeName = "icon";

        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
            => new IconProtocol();
    }
}
