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
        public Items(IEnumerable<DirectoryItem> directories, IEnumerable<FileItem> files)
        {
            ViewType = ViewType.Directory;
            Drives = null;
            Directories = directories.ToArray();
            Files = files.ToArray();
        }

        public Items(IEnumerable<Drive> drives)
        {
            ViewType = ViewType.Root;
            Drives = drives.ToArray();
            Directories = null;
            Files = null;
        }

        public ViewType ViewType { get; }
        public Drive[] Drives { get; }
        public DirectoryItem[] Directories { get; }
        public FileItem[] Files { get; }
    }
}
