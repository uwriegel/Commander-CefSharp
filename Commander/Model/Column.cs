using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commander.Enums;

namespace Commander.Model
{
    struct Column
    {
        public Column(string name, bool isSortable = true, ColumnsType columnsType = ColumnsType.String)
        {
            Name = name;
            IsSortable = isSortable;
            ColumnsType = columnsType;
        }

        public string Name { get; }
        public bool IsSortable { get; }
        public ColumnsType ColumnsType { get; }
    }
}

