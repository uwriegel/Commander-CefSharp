using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commander.Extension;
using Commander.Model;

namespace Commander.Processors
{
    static class DirectoryProcessor
    {
        public static string Name { get; } = "directory";

        public static Items Get(string path)
        {
            var di = new DirectoryInfo(path);
            var directories = GetSafeItems(() => di.GetDirectories());
            var files = GetSafeItems(() => di.GetFiles());
            return new Items(directories.Select(n => new DirectoryItem(n.Name, n.LastWriteTime, n.Attributes.HasFlag(FileAttributes.Hidden))),
                files.Select(n => new FileItem(n.Name, n.FullName, n.Extension, n.LastWriteTime, n.Length, n.Attributes.HasFlag(FileAttributes.Hidden))));
        }

        public static IEnumerable<ResponseItem> GetItems(Items items)
        {
            var directories = items.Directories.Select((n, i) => new ResponseItem(Enums.ItemType.Directory, i + 1,
                new[] {
                    n.Name,
                    "",
                    n.Date.ToString("g"),
                    "",
                    ""
                }, 
                "Folder", false, n.IsHidden)).ToArray();
            var files = items.Files.Select((n, i) => new ResponseItem(Enums.ItemType.File, i + 1 + directories.Length,
                new[] {
                    n.Name,
                    n.Extension,
                    n.Date.ToString("g"),
                    n.Size.ToString("N0")
                },
                n.Icon, false, n.IsHidden)).ToArray();

            return Enumerable.Repeat<ResponseItem>(new ResponseItem(Enums.ItemType.Parent, 0, new[] { ".." }, "Folder", false, false), 1)
                .Concat(directories)
                .Concat(files);
        }
        
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
    }
}
