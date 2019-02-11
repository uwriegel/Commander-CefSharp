using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Model
{
    struct Drive
    {
        public Drive(int index, string name, string label, long size)
        {
            Index = index;
            Name = name;
            Label = label;
            Size = size;
        }

        public int Index { get; }
        public string Name { get; }
        public string Label { get; }
        public long Size { get; }
    }
}
