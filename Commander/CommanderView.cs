﻿using System.Diagnostics;
using System.Threading.Tasks;

using CefSharp;
using CefSharp.WinForms;
using Commander.Properties;
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
            var viewType = GetViewType(path);
            var columns = GetColumns(viewType);
            await ExecuteScript("setColumns", columns);
            ChangePath(path);
        }

        public async void ChangePath(string path)
        {
            var viewType = GetViewType(path);
            if (viewType != currentViewType)
            {
                var columns = GetColumns(viewType);
                await ExecuteScript("setColumns", columns);
                currentViewType = viewType;
            }




            Enums.ViewType vt;
            switch (viewType)
            {
                case ViewType.Root:
                    vt = Enums.ViewType.Root;
                    break;
                default:
                    vt = Enums.ViewType.Directory;
                    break;
            }



            // TODO: return computed path
            currentItems = await ThreadTask<ResponseItem[]>.RunAsync(() => Engine.Get(vt, path));
            host.RecentPath = path;
            await ExecuteScriptWithParams("itemsChanged", currentItems.Length);
        }

        public string GetItems(int start, int end)
        {
            return Json.Serialize(currentItems);
        }

        Columns GetColumns(ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.Root:
                    return new Columns(Root.Name, new[]
                    {
                        new Column(Resources.RootName, true),
                        new Column(Resources.RootLabel, true),
                        new Column(Resources.RootSize, true)
                    });
                default:
                    return new Columns(Directory.Name, new[]
                    {
                        new Column(Resources.DirectoryName, true),
                        new Column(Resources.DirectoryExtension, true),
                        new Column(Resources.DirectoryDate, true),
                        new Column(Resources.DirectorySize, true),
                        new Column(Resources.DirectoryVersion, true)
                    });
            }
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

        ViewType GetViewType(string path)
        {
            if (path == Root.Name)
                return ViewType.Root;
            else
                return ViewType.Directory;
        }

        enum ViewType
        {
            Root,
            Directory
        }

        readonly ID id;
        readonly ChromiumWebBrowser browser;
        readonly IHost host;
        ViewType currentViewType;
        ResponseItem[] currentItems;
    }
}
