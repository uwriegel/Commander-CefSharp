using System.Diagnostics;
using System.Threading.Tasks;

using CefSharp;
using CefSharp.WinForms;

using static Enums;
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
            await GetColumns(viewType);
            ChangePath(path);
        }

        public async void ChangePath(string path)
        {
            var viewType = Engine.GetViewType(path);
            if (viewType != currentViewType)
            {
                await GetColumns(viewType);
                currentViewType = viewType;
            }
            currentItems = await ThreadTask<ResponseItem[]>.RunAsync(() => Engine.Get(viewType, path));
            await ExecuteScriptWithParams("itemsChanged", currentItems.Length);
        }

        public string GetItems(int start, int end)
        {
            return Json.Serialize(currentItems);
        }

        async Task GetColumns(ViewType viewType)
        {
            var columns = Engine.GetColumns(viewType);
            await ExecuteScript("setColumns", columns);
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

        async Task ExecuteScriptWithParams(string method, params object[] parameters)
        {
            var sw = new Stopwatch();
            sw.Start();

            await browser.EvaluateScriptAsync($"{host.Class}.{method}", parameters);

            var elapsed = sw.Elapsed;
            Debugger.Log(1, "Main", $"Script execution duration: {elapsed}");
        }

        readonly ID id;
        readonly ChromiumWebBrowser browser;
        readonly IHost host;
        ViewType currentViewType;
        ResponseItem[] currentItems;
    }
}
