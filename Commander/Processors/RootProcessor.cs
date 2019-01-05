using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commander.Model;

namespace Commander.Processors
{
    static class RootProcessor
    {
        public static string Name { get; } = "root";

        public static Items Get()
        {
            var drives = DriveInfo.GetDrives()
                .Where(d => d.IsReady)
                .OrderBy(d => d.Name)
                .Select((n, i) => new Drive(i, n.Name, n.VolumeLabel, n.TotalSize));
            return new Items(drives);
        }
    }
}
