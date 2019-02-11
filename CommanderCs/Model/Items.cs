using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commander.Enums;

namespace Commander.Model
{
    struct Items
    {
        public static Items UpdateFiles(Items itemsToUpdate, IEnumerable<FileItem> files)
            => new Items(itemsToUpdate, files);

        public Items(string path, IEnumerable<DirectoryItem> directories, IEnumerable<FileItem> files)
        {
            Path = path;
            ViewType = ViewType.Directory;
            Drives = null;
            Directories = directories.ToArray();
            Files = files.ToArray();
        }

        public Items(IEnumerable<Drive> drives)
        {
            Path = "root";
            ViewType = ViewType.Root;
            Drives = drives.ToArray();
            Directories = null;
            Files = null;
        }

        Items(Items itemsToUpdate, IEnumerable<FileItem> files)
        {
            Path = itemsToUpdate.Path;
            ViewType = ViewType.Directory;
            Drives = null;
            Directories = itemsToUpdate.Directories;
            Files = files.ToArray();
        }

        public ViewType ViewType { get; }
        public string Path { get; }
        public Drive[] Drives { get; }
        public DirectoryItem[] Directories { get; }
        public FileItem[] Files { get; }
    }
}
