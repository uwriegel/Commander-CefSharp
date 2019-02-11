using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Extension
{
    static class StringExtension
    {
        public static string GetNameOnly(this string name)
        {
            var pos = name.LastIndexOf(".");
            return pos != -1 ? name.Substring(0, pos) : name;
        }
    }
}
