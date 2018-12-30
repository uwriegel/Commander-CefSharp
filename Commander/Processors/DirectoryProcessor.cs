﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commander.Enums;
using Commander.Extension;
using Commander.Model;
using static Commander.ExifReader;

namespace Commander.Processors
{
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
            => Enumerable.Repeat<ResponseItem>(new ResponseItem(Enums.ItemType.Parent, 0, 
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
    }
}
