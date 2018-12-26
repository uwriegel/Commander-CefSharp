using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp.WinForms;
using CefSharp;
using static Model;

namespace Commander
{
    class CommanderView
    {
        public enum ID
        {
            Left,
            Right
        }

        public CommanderView(ID id, ChromiumWebBrowser browser, IHost host)
        {
            this.id = id;
            this.browser = browser;
            this.host = host;
        }

        public async void Ready()
        {
            var path = host.RecentPath;
            var viewType = Engine.GetViewType(path);
            var columns = Engine.GetColumns(viewType);
            await ExecuteScript("setColumns", columns);
        }

        public async void Get(string path)
        {
            var viewType = Engine.GetViewType(path);
            var result = await ThreadTask<ResponseItem[]>.RunAsync(() => Engine.Get(viewType, path));
        }

        void ChangePath(string path)
        {

        }

        async Task ExecuteScript(string method, object param)
        {
            var sw = new Stopwatch();
            sw.Start();

            var json = Json.Serialize(param);
            await browser.EvaluateScriptAsync($"{host.Class}.{method}({json})");

            var elapsed = sw.Elapsed;
            Debugger.Log(1, "Main", $"Script execution duration: {elapsed}");
        }

        readonly ID id;
        readonly ChromiumWebBrowser browser;
        readonly IHost host;
    }
}
