using System;
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
        #region Properties

        public bool ShowHidden
        {
            get => _showHidden;
            set
            {
                _showHidden = value;
                Refresh();
            }
        }
        bool _showHidden;

        #endregion

        #region Types

        public enum ID
        {
            Left,
            Right
        }

        #endregion

        #region Constructor

        public CommanderView(ID id, ChromiumWebBrowser browser, IHost host)
        {
            this.id = id;
            this.browser = browser;
            this.host = host;
        }

        #endregion

        #region Interface

        public async void Ready()
        {
            var path = host.RecentPath;
            var viewType = GetViewType(path);
            var columns = GetColumns(viewType);
            await ExecuteScriptAsync("setColumns", columns);
            ChangePath(path);
        }

        public async void ChangePath(string path)
        {
            if (path == null)
                return;

            var request = requestFactory.Create();

            var viewType = GetViewType(path);
            var setColumns = viewType != currentItems.ViewType;

            var newItems = await Task.Factory.StartNew(() => 
            {
                switch (viewType)
                {
                    case ViewType.Root:
                        currentSorting = null;
                        return RootProcessor.Get();
                    default:
                        return DirectoryProcessor.Get(path, ShowHidden);
                }
            });

            if (request.IsCancelled)
                return;

            if (setColumns)
            {
                var columns = GetColumns(viewType);
                await ExecuteScriptAsync("setColumns", columns);
            }

            currentItems = newItems;

            Sort();

            host.RecentPath = currentItems.Path;
            currentIndex = ItemIndex.GetDefault(currentItems.ViewType);
            await ExecuteScriptWithParams("itemsChanged");

            if (currentItems.ViewType == ViewType.Directory)
            {
                var extended = await Task.Factory.StartNew(() => currentItems.ExtendItems());

                if (request.IsCancelled)
                    return;

                currentItems = extended;
                Sort();

                if (request.IsCancelled)
                    return;

                await ExecuteScriptWithParams("itemsChanged");
            }
        }

        public string GetItems()
        {
            var sw = new Stopwatch();
            sw.Start();

            IEnumerable<ResponseItem> resultItems;
            switch (currentItems.ViewType)
            {
                case ViewType.Root:
                    resultItems = currentItems.Drives.Select(n => 
                        new ResponseItem(ItemType.Directory, ItemIndex.Create(ItemType.Directory, n.Index), 
                        new[] {
                            n.Name,
                            n.Label,
                            n.Size.ToString("N0")
                        }, "Drive", currentIndex.IsSelected(n.Index, ItemType.Directory)));
                    break;
                default:
                    resultItems = DirectoryProcessor.GetItems(currentItems, currentIndex);
                    break;
            }

            var response = new Response(currentItems.Path, resultItems);
            var result = Json.Serialize(response);
            var elapsed = sw.Elapsed;
            Debugger.Log(1, "Main", $"JSON conversion duration: {elapsed}");
            return result;
        }

        public void SetIndex(int index) => currentIndex = index;

        public async void Sort(int index, bool ascending)
        {
            currentSorting = (index, !ascending);
            Sort();
            await ExecuteScriptWithParams("itemsChanged");
        }

        #endregion

        #region Methods

        public void Refresh() => ChangePath(currentItems.Path);

        Columns GetColumns(ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.Root:
                    return new Columns(RootProcessor.Name, new[]
                    {
                        new Column(Resources.RootName, false),
                        new Column(Resources.RootLabel, false),
                        new Column(Resources.RootSize, false, ColumnsType.Size)
                    });
                default:
                    return new Columns(DirectoryProcessor.Name, new[]
                    {
                        new Column(Resources.DirectoryName),
                        new Column(Resources.DirectoryExtension),
                        new Column(Resources.DirectoryDate, true, ColumnsType.Date),
                        new Column(Resources.DirectorySize, true, ColumnsType.Size),
                        new Column(Resources.DirectoryVersion)
                    });
            }
        }

        async Task ExecuteScriptAsync(string method, object param)
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

        void Sort()
        {
            if (currentSorting.HasValue && currentItems.Files != null)
            {
                IEnumerable<FileItem> sortedItems;
                switch (currentSorting.Value.index)
                {
                    case 0:
                        sortedItems = currentItems.Files.OrderBy(n => n.Name);
                        break;
                    case 1:
                        sortedItems = currentItems.Files.OrderBy(n => n.Extension).ThenBy(n => n.Name);
                        break;
                    case 2:
                        sortedItems = currentItems.Files.OrderBy(n => n.Date).ThenBy(n => n.Name);
                        break;
                    case 3:
                        sortedItems = currentItems.Files.OrderBy(n => n.Size).ThenBy(n => n.Name);
                        break;
                    case 4:
                        sortedItems = currentItems.Files.OrderBy(n => n.Version, FileVersion.Comparer).ThenBy(n => n.Name);
                        break;
                    default:
                        sortedItems = currentItems.Files;
                        break;
                }
                if (currentSorting.Value.descending)
                    sortedItems = sortedItems.Reverse();

                currentItems = Items.UpdateFiles(currentItems, sortedItems);
            }
        }

        #endregion

        #region Fields

        readonly ID id;
        readonly ChromiumWebBrowser browser;
        readonly IHost host;
        readonly RequestFactory requestFactory = new RequestFactory();
        
        Items currentItems;
        int currentIndex;

        (int index, bool descending)? currentSorting = null;

        #endregion
    }
}
