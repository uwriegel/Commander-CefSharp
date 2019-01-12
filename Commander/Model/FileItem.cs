﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commander.Extension;

namespace Commander.Model
{
    struct FileItem
    {
        public static FileItem UpdateDate(FileItem itemToUpdate, DateTime date) => new FileItem(itemToUpdate, date);

        public static FileItem UpdateVersion(FileItem itemToUpdate, FileVersionInfo version) => new FileItem(itemToUpdate, version);

        public FileItem(int index, string name, string fullname, string extension, DateTime date, long size, bool isHidden)
        {
            Index = index;
            Name = name.GetNameOnly();
            Extension = extension;
            Date = date;
            HasExifDate = false;
            Size = size;
            IsHidden = isHidden;
            Version = null;

            Icon = (Program.IsAngularServing ? "serve://commander/" : "") +
                (string.Compare(extension, ".exe", true) == 0 ? "icon?path=" + fullname : "icon?path=" + extension);
        }

        FileItem(FileItem itemToUpdate, DateTime date)
        {
            Index = itemToUpdate.Index;
            Name = itemToUpdate.Name;
            Extension = itemToUpdate.Extension;
            HasExifDate = true;
            Date = date;
            Size = itemToUpdate.Size;
            IsHidden = itemToUpdate.IsHidden;
            Icon = itemToUpdate.Icon;
            Version = null;
        }

        FileItem(FileItem itemToUpdate, FileVersionInfo version)
        {
            Index = itemToUpdate.Index;
            Name = itemToUpdate.Name;
            Extension = itemToUpdate.Extension;
            HasExifDate = false;
            Date = itemToUpdate.Date;
            Size = itemToUpdate.Size;
            IsHidden = itemToUpdate.IsHidden;
            Icon = itemToUpdate.Icon;
            Version = version;
        }

        public int Index { get; }
        public string Name { get; }
        public string Extension { get; }
        public string Icon { get; }
        public DateTime Date { get; }
        public long Size { get; }
        public bool IsHidden { get; }
        public bool HasExifDate { get; }
        public FileVersionInfo Version { get; }
    }
}
