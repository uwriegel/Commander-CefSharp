using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.Menu;

namespace Commander.Extension
{
    static class MenuItemCollectionExtensions
    {
        public static IEnumerable<MenuItem> ToIEnumerable(this MenuItemCollection menuItemCollection)
        {
            var enumerator = menuItemCollection?.GetEnumerator();
            if (enumerator == null)
                yield break;
            while (enumerator.MoveNext())
                yield return enumerator.Current as MenuItem;
        }
    }
}
