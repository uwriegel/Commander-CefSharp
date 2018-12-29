using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Model
{
    struct DirectoryItem
    {
        public DirectoryItem(string name, DateTime date, bool isHidden)
        {
            Name = name;
            Date = date;
            IsHidden = isHidden;
        }

        public string Name { get; }
        public DateTime Date { get; }
        public bool IsHidden { get; }
    }
}
