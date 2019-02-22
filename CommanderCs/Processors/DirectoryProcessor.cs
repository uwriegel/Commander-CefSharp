using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Commander.Enums;
using Commander.Exceptions;
using Commander.Extension;
using Commander.Model;
using static Commander.ExifReader;

namespace Commander.Processors
{
    // Error copying: Show MessageBox
    static class DirectoryProcessor
    {
        public static string Name { get; } = "directory";

        public static Items Get(string path, bool showHidden)
        {
            var di = new DirectoryInfo(path);
            var directories =
                GetSafeItems(() => di.GetDirectories())
                .Where(n => showHidden ? true : !n.Attributes.HasFlag(FileAttributes.Hidden))
                .Select((n, i) => new DirectoryItem(i, n.Name, n.LastWriteTime, n.Attributes.HasFlag(FileAttributes.Hidden)));

            var files =
                GetSafeItems(() => di.GetFiles())
                .Where(n => showHidden ? true : !n.Attributes.HasFlag(FileAttributes.Hidden))
                .Select((n, i) => new FileItem(i, n.Name, n.FullName, n.Extension, n.LastWriteTime, n.Length, n.Attributes.HasFlag(FileAttributes.Hidden)));

            return new Items(di.FullName, directories, files);
        }

        public static Items ExtendItems(this Items itemsToExtend) =>
            Items.UpdateFiles(itemsToExtend, itemsToExtend.Files.Select(n => n.ExtendItems(itemsToExtend.Path)));

        public static IEnumerable<ResponseItem> GetItems(Items items, int currentIndex)
            => Enumerable.Repeat<ResponseItem>(new ResponseItem(Enums.ItemType.Parent, ItemIndex.Create(ItemType.Parent, 0),
                new[]
                {
                    ".."
                }, "Folder", currentIndex.IsSelected(0, ItemType.Parent)), 1)
                .Concat(items.Directories.Select(n => new ResponseItem(Enums.ItemType.Directory, ItemIndex.Create(ItemType.Directory, n.Index),
                new[] {
                    n.Name,
                    "",
                    n.Date.ToString("g")
                },
                "Folder", currentIndex.IsSelected(n.Index, ItemType.Directory), n.IsHidden)))
                .Concat(items.Files.Select(n => new ResponseItem(Enums.ItemType.File, ItemIndex.Create(ItemType.File, n.Index),
                new[] {
                    n.Name,
                    n.Extension,
                    n.Date.ToString("g"),
                    n.Size.ToString("N0"),
                    n.Version.GetVersion()
                },
                n.Icon, currentIndex.IsSelected(n.Index, ItemType.File), n.IsHidden, n.HasExifDate)));

        public static void CreateFolder(string path, string name)
        {
            var newFolder = Path.Combine(path, name);
            if (Directory.Exists(newFolder))
                throw new AlreadyExistsException();
            try
            {
                Directory.CreateDirectory(newFolder);
            }
            catch (UnauthorizedAccessException)
            {
                // TODO: 
            }
        }

        public static async Task<bool> Copy(this Items currentItems, IEnumerable<(int index, ItemType Type)> selectedItems, string targetPath, IntPtr mainWindow, Control dispatcher)
        {
            var pathes = selectedItems
                .Select(n => currentItems.GetItemPathes(n, targetPath))
                .Where(n => n.HasValue)
                .Select(n => n.Value);

            var fileop = new SHFILEOPSTRUCT()
            {
                fFlags = FILEOP_FLAGS.FOF_NOCONFIRMATION | FILEOP_FLAGS.FOF_NOCONFIRMMKDIR | FILEOP_FLAGS.FOF_MULTIDESTFILES,
                hwnd = mainWindow,
                lpszProgressTitle = "Commander",
                wFunc = FileFuncFlags.FO_COPY,
                pFrom = CreateFileOperationPaths(pathes.Select(n => n.sourcePath)),
                pTo = CreateFileOperationPaths(pathes.Select(n => n.targetPath))
            };
            // Wait till animation has finished
            return await dispatcher.DeferredExecution(() => Api.SHFileOperation(ref fileop) == 0, 400);
        }

        public static string[] GetTestItems() =>
            (from n in (new DirectoryInfo(@"c:\windows\system32")).GetFiles()
             let icon = (Program.IsAngularServing ? "serve://commander/" : "") +
                 (string.Compare(n.Extension, ".exe", true) == 0 ? "icon?path=" + n.FullName : "icon?path=" + n.Extension)
             select icon).ToArray();

        static IEnumerable<T> GetSafeItems<T>(Func<IEnumerable<T>> get) where T : FileSystemInfo
        {
            try
            {
                return get();
            }
            catch (UnauthorizedAccessException)
            {
                return Enumerable.Empty<T>();
            }
        }

        static FileItem ExtendItems(this FileItem itemToExtend, string path)
        {
            if (string.Compare(itemToExtend.Extension, ".tif", true) == 0
                || string.Compare(itemToExtend.Extension, ".jpeg", true) == 0
                || string.Compare(itemToExtend.Extension, ".jpg", true) == 0)
                return itemToExtend.UpdateExif(path);
            else if (string.Compare(itemToExtend.Extension, ".exe", true) == 0
                || string.Compare(itemToExtend.Extension, ".dll", true) == 0)
                return itemToExtend.UpdateVersion(path);
            else
                return itemToExtend;
        }

        static string GetFullName(this FileItem file, string path) => Path.Combine(path, file.Name + file.Extension);

        static FileItem UpdateVersion(this FileItem file, string path)
        {
            var fvi = FileVersionInfo.GetVersionInfo(file.GetFullName(path));
            return fvi.HasInfo() ? FileItem.UpdateVersion(file, fvi) : file;
        }

        static FileItem UpdateExif(this FileItem file, string path)
        {
            using (var reader = new ExifReader(file.GetFullName(path)))
                if (reader.GetTagValue<DateTime>(ExifTags.DateTimeOriginal, out var date))
                    return FileItem.UpdateDate(file, date);
                else
                    return file;
        }

        static string CreateFileOperationPaths(IEnumerable<string> paths)
        {
            var sb = new StringBuilder();
            foreach (var path in paths)
            {
                sb.Append(path);
                sb.Append("\x0");
            }
            sb.Append("\x0");
            return sb.ToString();
        }

        static (string sourcePath, string targetPath)? GetItemPathes(this Items currentItems, (int index, ItemType Type) item, string targetPath)
        {
            switch (item.Type)
            {
                case ItemType.File:
                    return (currentItems.Files[item.index].GetFullName(currentItems.Path),
                        Path.Combine(targetPath, currentItems.Files[item.index].Name + "." + currentItems.Files[item.index].Extension));
                case ItemType.Directory:
                    return (Path.Combine(currentItems.Path, currentItems.Directories[item.index].Name),
                        Path.Combine(targetPath, currentItems.Directories[item.index].Name));
                default:
                    return null;
            }
        }

        static IEnumerable<int> GetConflicts(IEnumerable<(string sourcePath, string targetPath)> pathes)
        {
            var items = pathes.Where(n => File.Exists(n.targetPath));
            return null;
        }
    }
}
