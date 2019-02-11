using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    interface IHost
    {
        string RecentPath { get; set; }
        string Class { get; }
    }
}
