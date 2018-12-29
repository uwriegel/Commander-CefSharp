using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commander.Model;

namespace Commander.Extension
{
    static class ItemsExtensions
    {
        public static int GetLength(this Items items)
        {
            if (items.Drives != null)
                return items.Drives.Count();
            else
                return 1 + items.Directories.Count() + items.Files.Count();
        }
    }
}
