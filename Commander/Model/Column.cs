using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Model
{
    struct Column
    {
        public Column(string name, bool isSortable = true)
        {
            Name = name;
            IsSortable = isSortable;
        }

        public string Name { get; }
        public bool IsSortable { get; }
    }
}

