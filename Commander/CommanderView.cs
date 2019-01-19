using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

using Commander.Enums;
using Commander.Exceptions;
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

        public string Path { get => currentItems.Path; }

        #endregion

        #region Types

        public enum ID
        {
            Left,
            Right
        }

        #endregion

        #region Constructor

        public CommanderView(ID id, IntPtr mainWindow, ChromiumWebBrowser browser, IHost host)
        {
            this.id = id;
            this.mainWindow = mainWindow;
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

        public async void ChangePath(string path, string directoryToSelect = null)
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

            int GetCurrentIndex()
            {
                if (directoryToSelect == null)
                    return ItemIndex.GetDefault(currentItems.ViewType);
                else if (viewType == ViewType.Directory)
                {
                    var folderToSelect = newItems.Directories.First(n => string.Compare(n.Name, directoryToSelect, true) == 0);
                    return ItemIndex.Create(ItemType.Directory, folderToSelect.Index);
                }
                else if (viewType == ViewType.Root)
                {
                    var folderToSelect = newItems.Drives.First(n => string.Compare(n.Name, directoryToSelect, true) == 0);
                    return ItemIndex.Create(ItemType.Directory, folderToSelect.Index);
                }
                else
                    return 0;
            }
            SetIndex(GetCurrentIndex());

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

        public void ProcessItem(ProcessItemType type)
        {
            var itemType = ItemIndex.GetItemType(currentIndex);
            var item = GetCurrentItemPath(currentIndex);

            switch (itemType)
            {
                case ItemType.Parent:
                    var info = new DirectoryInfo(currentItems.Path);
                    ChangePath(item, info.Name);
                    break;
                case ItemType.Directory:
                    if (type == ProcessItemType.Properties)
                        ProcessFile(item, type);
                    else
                        ChangePath(item);
                    break;
                case ItemType.File:
                    ProcessFile(item, type);
                    break;
                default:
                    break;
            }
        }

        void ProcessFile(string file, ProcessItemType type)
        {
            switch (type)
            {
                case ProcessItemType.Show:
                    var p = new Process();
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.ErrorDialog = true;
                    p.StartInfo.FileName = file;
                    p.Start();
                    break;
                case ProcessItemType.Properties:
                    var info = new ShellExecuteInfo()
                    {
                        lpVerb = "properties",
                        lpFile = file,
                        nShow = Api.SW_SHOW,
                        fMask = Api.SEE_MASK_INVOKEIDLIST
                    };
                    info.cbSize = Marshal.SizeOf(info);
                    Api.ShellExecuteEx(ref info);
                    break;
                case ProcessItemType.StartAs:
                    p = Process.Start("rundll32.exe", $"shell32, OpenAs_RunDLL {file}");
                    break;
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
            Debugger.Log(1, "Main", $"JSON conversion duration: {elapsed}\n");
            return result;
        }

        public async void SetIndex(int index)
        {
            currentIndex = index;
            await ExecuteScriptAsync("setCurrentItem", GetCurrentItemPath(currentIndex));
        }

        public async void Sort(int index, bool ascending)
        {
            currentSorting = (index, !ascending);
            Sort();
            await ExecuteScriptWithParams("itemsChanged");
        }

        public async void CreateFolder(string item)
        {
            try
            {
                var viewType = GetViewType(Path);
                switch (viewType)
                {
                    case ViewType.Directory:
                        DirectoryProcessor.CreateFolder(Path, item);
                        break;
                    default:
                        break;
                }
                ChangePath(Path, item);
            }
            catch (AlreadyExistsException)
            {
                await browser.EvaluateScriptAsync($"commander.showDialog", Resources.FolderAlreadyExists);
            }
        }

        public async void Copy(string targetPath)
        {
            var fileop = new SHFILEOPSTRUCT()
            {
                fFlags = FILEOP_FLAGS.FOF_NOCONFIRMATION | FILEOP_FLAGS.FOF_NOCONFIRMMKDIR | FILEOP_FLAGS.FOF_MULTIDESTFILES,
                hwnd = mainWindow,
                lpszProgressTitle = "Commander",
                wFunc = FileFuncFlags.FO_COPY,
            };
        }

        public void SetSelected(object[] objects)
            => selectedIndexes = objects?.Cast<int>().ToArray();

        #endregion

        #region Methods

        public void Refresh() => ChangePath(currentItems.Path);

        // TODO: call only in the right folder
        // TODO: Dialog resources (Abbrechen) 
        public async Task CreateFolder()
            //TODO: dialog.inputText = item.items[0] != ".." ? item.items[0] : ""
            => await ExecuteScriptWithParams("createFolder", Resources.dialogCreateFolder);

        public async Task Copy(CommanderView otherView)
            => await ExecuteScriptWithParams("copy", otherView.Path, Resources.dialogCopy);

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
            Debugger.Log(1, "Main", $"Script execution duration: {elapsed}, {elapsed2}\n");
        }

        string GetCurrentItemPath(int index)
        {
            var itemType = ItemIndex.GetItemType(index);
            var arrayIndex = ItemIndex.GetArrayIndex(index);

            string GetDirectoryItemPath()
            {
                switch (itemType)
                {
                    case ItemType.Directory:
                        return currentItems.Directories[arrayIndex].Name;
                    case ItemType.File:
                        return currentItems.Files[arrayIndex].Name + currentItems.Files[arrayIndex].Extension;
                    case ItemType.Parent:
                        var info = new DirectoryInfo(currentItems.Path);
                        return info.Parent?.FullName ?? "root";
                    default:
                        return null;
                }
            }

            string GetDirectory()
            {
                var directory = GetDirectoryItemPath();
                return directory == "root" ? "root" : System.IO.Path.Combine(currentItems.Path, directory);
            }

            return currentItems.ViewType == ViewType.Root 
                ? currentItems.Drives[arrayIndex].Name 
                : GetDirectory();
        }

        async Task ExecuteScriptWithParams(string method, params object[] parameters)
        {
            var sw = new Stopwatch();
            sw.Start();

            await browser.EvaluateScriptAsync($"{host.Class}.{method}", parameters);

            var elapsed = sw.Elapsed;
            Debugger.Log(1, "Main", $"Script execution duration: {elapsed}\n");
        }

        ViewType GetViewType(string path)
        {
            if (path == RootProcessor.Name || (path.EndsWith("..") && path.Length == 5))
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
        readonly IntPtr mainWindow;
        
        Items currentItems;
        int currentIndex;

        (int index, bool descending)? currentSorting = null;
        int[] selectedIndexes;

        #endregion
    }
}
