using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Model
{
    struct Columns
    {
        public Columns(string name, IEnumerable<Column> values)
        {
            Name = name;
            Values = values;
        }

        public string Name { get; }
        public IEnumerable<Column> Values { get; }

    }
}
