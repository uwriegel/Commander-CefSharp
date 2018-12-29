using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Model
{
    struct Drive
    {
        public Drive(string name, string label, long size)
        {
            Name = name;
            Label = label;
            Size = size;
        }

        public string Name { get; }
        public string Label { get; }
        public long Size { get; }
    }
}
