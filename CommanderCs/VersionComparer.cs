using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    class VersionComparer : IComparer<FileVersionInfo>
    {
        public int Compare(FileVersionInfo x, FileVersionInfo y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null)
                return -1;
            if (y == null)
                return 1;
            var result = x.FileMajorPart - y.FileMajorPart;
            if (result != 0)
                return result;
            result = x.FileMinorPart - y.FileMinorPart;
            if (result != 0)
                return result;
            result = x.FileBuildPart - y.FileBuildPart;
            if (result != 0)
                return result;
            return x.FilePrivatePart - y.FilePrivatePart;
        }
    }
}
