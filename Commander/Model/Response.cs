using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Model
{
    struct Response
    {
        public Response(string path, IEnumerable<ResponseItem> items)
        {
            Path = path;
            Items = items;
        }

        //string ItemToSelect
        public string Path { get; }
        public IEnumerable<ResponseItem> Items { get; }
    }
}
