using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commander.Extension;

namespace Commander.Model
{
    struct FileItem
    {
        public static FileItem UpdateDate(FileItem itemToUpdate, DateTime date) => new FileItem(itemToUpdate, date);
        public FileItem(string name, string fullname, string extension, DateTime date, long size, bool isHidden)
        {
            Name = name.GetNameOnly();
            Extension = extension;
            Date = date;
            HasExifDate = false;
            Size = size;
            IsHidden = isHidden;

            if (string.Compare(extension, ".exe", true) == 0)
                Icon = "icon?path=" + fullname;
            else
                Icon = "icon?path=" + extension;
        }

        FileItem(FileItem itemToUpdate, DateTime date)
        {
            Name = itemToUpdate.Name;
            Extension = itemToUpdate.Extension;
            HasExifDate = true;
            Date = date;
            Size = itemToUpdate.Size;
            IsHidden = itemToUpdate.IsHidden;
            Icon = itemToUpdate.Icon;
        }

        public string Name { get; }
        public string Extension { get; }
        public string Icon { get; }
        public DateTime Date { get; }
        public long Size { get; }
        public bool IsHidden { get; }
        public bool HasExifDate { get; }
    }
}
