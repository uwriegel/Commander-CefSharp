using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using CefSharp;
using CefSharp.WinForms;

using Commander.Enums;
using Commander.Extension;
using Commander.Model;
using Commander.Processors;
using Commander.Properties;

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
            var viewType = GetViewType(path);
            var columns = GetColumns(viewType);
            await ExecuteScript("setColumns", columns);
            ChangePath(path);
        }

        public async void ChangePath(string path)
        {
            var viewType = GetViewType(path);
            var setColumns = viewType != currentItems.ViewType;

            var length = await Task.Factory.StartNew(() => 
            {
                switch (viewType)
                {
                    case ViewType.Root:
                        currentItems = RootProcessor.Get();
                        break;
                    default:
                        currentItems = DirectoryProcessor.Get(path);
                        break;
                }
                return currentItems.GetLength();
            });

            if (setColumns)
            {
                var columns = GetColumns(viewType);
                await ExecuteScript("setColumns", columns);
            }

            host.RecentPath = currentItems.Path;
            await ExecuteScriptWithParams("itemsChanged", length);
        }

        public string GetItems()
        {
            var sw = new Stopwatch();
            sw.Start();

            IEnumerable<ResponseItem> resultItems;
            switch (currentItems.ViewType)
            {
                case ViewType.Root:
                    resultItems = currentItems.Drives.Select((n, i) => 
                        new ResponseItem(ItemType.Directory, IndexOperations.CombineIndexes((byte)ItemType.Directory, i), 
                        new[] {
                            n.Name,
                            n.Label,
                            n.Size.ToString("N0")
                        }, "Drive", false, false));
                    break;
                default:
                    resultItems = DirectoryProcessor.GetItems(currentItems);
                    break;
            }

            var response = new Response(currentItems.Path, resultItems);
            var result = Json.Serialize(response);
            var elapsed = sw.Elapsed;
            Debugger.Log(1, "Main", $"JSON conversion duration: {elapsed}");
            return result;
        }

        Columns GetColumns(ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.Root:
                    return new Columns(RootProcessor.Name, new[]
                    {
                        new Column(Resources.RootName),
                        new Column(Resources.RootLabel),
                        new Column(Resources.RootSize, false, ColumnsType.Size)
                    });
                default:
                    return new Columns(DirectoryProcessor.Name, new[]
                    {
                        new Column(Resources.DirectoryName, true),
                        new Column(Resources.DirectoryExtension, true),
                        new Column(Resources.DirectoryDate, true, ColumnsType.Date),
                        new Column(Resources.DirectorySize, true, ColumnsType.Size),
                        new Column(Resources.DirectoryVersion, true)
                    });
            }
        }
        async Task ExecuteScript(string method, object param)
        {
            var sw = new Stopwatch();
            sw.Start();

            var json = Json.Serialize(param);
            var elapsed = sw.Elapsed;
            await browser.EvaluateScriptAsync($"{host.Class}.{method}({json})");

            var elapsed2 = sw.Elapsed;
            Debugger.Log(1, "Main", $"Script execution duration: {elapsed}, {elapsed2}");
        }

        async Task ExecuteScriptWithParams(string method, params object[] parameters)
        {
            var sw = new Stopwatch();
            sw.Start();

            await browser.EvaluateScriptAsync($"{host.Class}.{method}", parameters);

            var elapsed = sw.Elapsed;
            Debugger.Log(1, "Main", $"Script execution duration: {elapsed}");
        }

        ViewType GetViewType(string path)
        {
            if (path == RootProcessor.Name)
                return ViewType.Root;
            else
                return ViewType.Directory;
        }

        readonly ID id;
        readonly ChromiumWebBrowser browser;
        readonly IHost host;
        
        Items currentItems;
    }
}
