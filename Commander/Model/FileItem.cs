using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commander.Extension;

namespace Commander.Model
{
    class FileItem
    {
        public FileItem(string name, string fullname, string extension, DateTime date, long size, bool isHidden)
        {
            Name = name.GetNameOnly();
            Extension = extension;
            Date = date;
            Size = size;
            IsHidden = isHidden;

            if (string.Compare(extension, ".exe", true) == 0)
                Icon = "icon?path=" + fullname;
            else
                Icon = "icon?path=" + extension;
        }

        public string Name { get; }
        public string Extension { get; }
        public string Icon { get; }
        public DateTime Date { get; }
        public long Size { get; }
        public bool IsHidden { get; }
    }
}
