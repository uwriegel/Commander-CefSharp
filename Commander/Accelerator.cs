using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Commander
{
    struct Accelerator
    {
        public Accelerator(MenuItem menuItem)
        {
            this.MenuItem = menuItem;
            switch (menuItem.Shortcut)
            {
                case Shortcut.F12:
                    Key = 123;
                    Alt = false;
                    Ctrl = false;
                    Shift = false;
                    break;
                default:
                    Key = 0;
                    Alt = false;
                    Ctrl = false;
                    Shift = false;
                    break;
            }
        }

        public MenuItem MenuItem;
        public int Key;
        public bool Alt;
        public bool Ctrl;
        public bool Shift;
    }
}
